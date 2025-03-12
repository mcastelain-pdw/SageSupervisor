// ServiceBrokerService.cs - Service d'arrière-plan pour Blazor
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using SageSupervisor.Models;
using SageSupervisor.Models.DTO;

namespace SageSupervisor.Services
{
    public class ServiceBrokerService(
        ILogger<ServiceBrokerService> logger, 
        IConfiguration configuration,
        IDbContextFactory<DataContext> contextFactory,
        ServiceBrokerMonitor monitor) : BackgroundService
    {
        private readonly ILogger<ServiceBrokerService> _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        private ServiceBrokerMonitor? _monitor = monitor;
        private readonly ConcurrentQueue<DocChangeEventArgs> _notificationDocQueue = new();
        private readonly ConcurrentQueue<TiersChangeEventArgs> _notificationTiersQueue = new();
        private readonly IDbContextFactory<DataContext> _contextFactory = contextFactory;
        
        // Événement que les composants Blazor peuvent écouter
        public event EventHandler<DocumentChangeDto>? DocTableChanged;
        public event EventHandler<TiersChangeDto>? TiersTableChanged;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service de surveillance des modifications de base de données démarré");

            try
            {
                // S'abonner aux événements de changement de document
                _monitor!.DocTableChanged += OnDocTableChanged!;
                _monitor!.TiersTableChanged += OnTiersTableChanged!;
                
                // Démarrer la surveillance
                _monitor.Start();

                // Maintenir le service en vie jusqu'à l'arrêt de l'application
                while (!stoppingToken.IsCancellationRequested)
                {
                    // Traiter les notifications en attente
                    ProcessNotificationQueue();
                    
                    await Task.Delay(500, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans le service de surveillance des modifications");
            }
            finally
            {
                // Nettoyage
                if (_monitor != null)
                {
                    _monitor.DocTableChanged -= OnDocTableChanged!;
                    _monitor.Stop();
                    _monitor.Dispose();
                }
                
                _logger.LogInformation("Service de surveillance des modifications de base de données arrêté");
            }
        }
        
        private void OnDocTableChanged(object sender, DocChangeEventArgs e)
        {
            using var cn = _contextFactory.CreateDbContext();
            _logger.LogInformation($"Evènement détecté: ID={e.RecordId}, Type={e.ChangeType}");

            // Test si message à traiter
            if (cn.DocumentChangeDtos.Where(t => t.NumPiece == e.RecordId && t.TotalHT != e.TotalHT).Any())
            {
                _logger.LogInformation($"Evènement ignoré: ID {e.RecordId} déjà en attente de traitement");
                return;
            }
            if (e.TotalHT == 0)
            {
                _logger.LogInformation($"Evènement ignoré: ID {e.RecordId} avec TotalHT=0");
                return;
            }

            // Mettre la notification dans une file d'attente pour traitement
            _notificationDocQueue.Enqueue(e);
            
            // Ajout en BDD
            cn.DocumentChangeDtos.Add(new DocumentChangeDto
            {
                NumPiece = e.RecordId,
                ChangeType = e.ChangeType,
                UpdatedDate = e.Timestamp,
                TotalHT = e.TotalHT,
                Domaine = (DocDomaineEnum)e.Domaine,
                Type = (DocTypeEnum)e.Type
            });
            cn.SaveChanges();
        }

        private void OnTiersTableChanged(object sender, TiersChangeEventArgs e)
        {
            using var cn = _contextFactory.CreateDbContext();
            _logger.LogInformation($"Evènement détecté: ID={e.RecordId}, Type={e.ChangeType}");

            // Test si message à traiter
            if (cn.TiersChangeDtos.Where(t => t.NumTiers == e.RecordId).Any())
            {
                _logger.LogInformation($"Evènement ignoré: ID {e.RecordId} déjà en attente de traitement");
                return;
            }

            // Mettre la notification dans une file d'attente pour traitement
            _notificationTiersQueue.Enqueue(e);
            
            // Ajout en BDD
            cn.TiersChangeDtos.Add(new TiersChangeDto
            {
                NumTiers = e.RecordId,
                ChangeType = e.ChangeType,
                UpdatedDate = e.Timestamp,
                Type = (TiersTypeEnum)e.Type
            });
            cn.SaveChanges();
        }
        
        private void ProcessNotificationQueue()
        {
            // Traiter toutes les notifications en attente
            while (_notificationDocQueue.TryDequeue(out var notification))
            {
                // Déclencher l'événement que les composants Blazor peuvent écouter
                DocTableChanged?.Invoke(this, new DocumentChangeDto
                {
                    NumPiece = notification.RecordId,
                    ChangeType = notification.ChangeType,
                    UpdatedDate = notification.Timestamp,
                    TotalHT = notification.TotalHT,
                    Domaine = (DocDomaineEnum)notification.Domaine,
                    Type = (DocTypeEnum)notification.Type
                });
            }

            while (_notificationTiersQueue.TryDequeue(out var notification))
            {
                // Déclencher l'événement que les composants Blazor peuvent écouter
                TiersTableChanged?.Invoke(this, new TiersChangeDto
                {
                    NumTiers = notification.RecordId,
                    ChangeType = notification.ChangeType,
                    UpdatedDate = notification.Timestamp,
                    Type = (TiersTypeEnum)notification.Type
                });
            }
        }
    }
}