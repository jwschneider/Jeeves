using System;
using System.Collections.Generic;
using System.Linq;

namespace Jeeves
{
    public static class Scheduler
    {
        public static IEnumerable<(Job, int)> Schedule(IEnumerable<Job> jobs, SetupTime setupTime)
        {
            return CreatePotentialSchedules(new DominatingPriorityQueue<ScheduleState>(ScheduleState.init()), jobs, setupTime)
                .MaxBy<ScheduleState, int>((ScheduleState state) => state.Value)
                .StateToSchedule();
        }
        
        public static IEnumerable<(Job, int)> StateToSchedule(this ScheduleState state)
        {
            return state == null ?
                new List<(Job, int)>() :
                StateToSchedule(state.Parent).Append<(Job, int)>((state.LastScheduled, state.TimeScheduled));
        }
        public static IEnumerable<ScheduleState> CreatePotentialSchedules(DominatingPriorityQueue<ScheduleState> PQ, IEnumerable<Job> jobs, SetupTime setupTime)
        {
            if (PQ.IsEmpty())
                return new List<ScheduleState>();
            else
            {
                ScheduleState head = PQ.PullHead();
                return head.HasChildren(jobs, setupTime) ?
                    CreatePotentialSchedules(PQ + head.Children(jobs, setupTime), jobs, setupTime) :
                    CreatePotentialSchedules(PQ, jobs, setupTime).Append<ScheduleState>(head);
                    
            }
        }
        public static bool HasChildren(this ScheduleState i, IEnumerable<Job> jobs, SetupTime setupTime)
        {
            return false;
        }
        public static IEnumerable<ScheduleState> Children(this ScheduleState i, IEnumerable<Job> jobs, SetupTime setupTime)
        {
            return null;
        }
        
    }

    public delegate int SetupTime(Job i, Job j);

    public class ScheduleState
    {
        public Job LastScheduled { get; }
        public IEnumerable<Job> Exclude { get; }
        public int TimeScheduled { get; }
        public int Value { get; }
        public ScheduleState Parent { get; }
        public ScheduleState(Job i, IEnumerable<Job> X, int t_i, int value, ScheduleState parent)
        {
            LastScheduled = i;
            Exclude = X;
            TimeScheduled = t_i;
            Value = value;
            Parent = parent;
        }
        public ScheduleState(Job i, IEnumerable<Job> X, int t_i) : this(i, X, t_i, 0, null)
        {

        }
        public static ScheduleState init()
        {
            return new ScheduleState(null, new List<Job>(), 0);
        }

    }

    public class ScheduleStateComparer : IComparer<ScheduleState>
    {
        public int Compare(ScheduleState i, ScheduleState j)
        {
            return i.Value.CompareTo(j.Value);
        }
    }
}
