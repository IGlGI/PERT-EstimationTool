using PertEstimationTool.Models;
using Prism.Events;
using System.Collections.ObjectModel;

namespace PertEstimationTool.Events
{
    public class TasksCollectionMessage : PubSubEvent<ObservableCollection<TaskItem>>
    {
    }
}
