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
        IDbContextFactory<DataContext> contextFactory) : BackgroundService
    {
        private readonly ILogger<ServiceBrokerService> _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        private ServiceBrokerMonitor? _monitor;
        private readonly ConcurrentQueue<TableChangeEventArgs> _notificationQueue = new();
        private readonly IDbContextFactory<DataContext> _contextFactory = contextFactory;
        
        // Événement que les composants Blazor peuvent écouter
        public event EventHandler<TableChangeEventArgs>? TableChanged;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service de surveillance des modifications de base de données démarré");

            try
            {
                // Récupérer la chaîne de connexion depuis la configuration
                string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "";
                
                // Initialiser le moniteur Service Broker
                _monitor = new ServiceBrokerMonitor(connectionString);
                
                // S'abonner aux événements de changement
                _monitor.TableChanged += OnTableChanged!;
                
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
                    _monitor.TableChanged -= OnTableChanged!;
                    _monitor.Stop();
                    _monitor.Dispose();
                }
                
                _logger.LogInformation("Service de surveillance des modifications de base de données arrêté");
            }
        }
        
        private void OnTableChanged(object sender, TableChangeEventArgs e)
        {
            // Mettre la notification dans une file d'attente pour traitement
            _notificationQueue.Enqueue(e);
            _logger.LogInformation($"Modification détectée: ID={e.RecordId}, Type={e.ChangeType}");

            // Ajout en BDD
            using var cn = _contextFactory.CreateDbContext();
            cn.TableChangeDtos.Add(new TableChangeDto
            {
                NumPiece = e.RecordId,
                ChangeType = e.ChangeType,
                UpdatedDate = e.Timestamp,
                Domaine = e.Domaine,
                Type = e.Type
            });
            cn.SaveChanges();
        }
        
        private void ProcessNotificationQueue()
        {
            // Traiter toutes les notifications en attente
            while (_notificationQueue.TryDequeue(out var notification))
            {
                // Déclencher l'événement que les composants Blazor peuvent écouter
                TableChanged?.Invoke(this, notification);
            }
        }
        
        // Méthode pour obtenir explicitement les dernières modifications
        public async Task<TableChangeEventArgs[]> GetRecentChangesAsync()
        {
            return await Task.FromResult<TableChangeEventArgs[]>([.. _notificationQueue]);
        }
    }
}