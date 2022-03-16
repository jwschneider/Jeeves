using System;
using System.Collections.Generic;
using System.Linq;

namespace Jeeves
{
    public static class Scheduler
    {
        public static IEnumerable<(Job, int)> Schedule(IEnumerable<Job> jobs, SetupTime setupTime)
        {
            return CreatePotentialSchedules(
                new DominatingPriorityQueue<ScheduleState>(
                    ScheduleState.init(),
                    (s1, s2) => s1.TimeScheduled - s2.TimeScheduled,
                    ScheduleStateDominator(setupTime)),
                jobs,
                setupTime)
                .GroupBy(state => state.Value)
                .MaxBy(group => group.Key)
                .MinBy(state => state.TimeScheduled + state.LastScheduled.ProcessTime)
                .StateToSchedule();
        }
        
        public static IEnumerable<(Job, int)> StateToSchedule(this ScheduleState state)
        {
            return state == null ?
                new List<(Job, int)>() :
                StateToSchedule(state.Parent).Append((state.LastScheduled, state.TimeScheduled));
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
                    CreatePotentialSchedules(PQ, jobs, setupTime).Append(head);
                    
            }
        }
        public static bool HasChildren(this ScheduleState s, IEnumerable<Job> jobs, SetupTime setup)
        {
            var (i, X, ti) = s;
            if (i == null) return jobs.Any();
            else
            {
                var F = RemainingCandidates(jobs, setup);
                return F(i, ti).Except(X).Any();
            }
        }

        public static IEnumerable<ScheduleState> Children(this ScheduleState s, IEnumerable<Job> jobs, SetupTime setup)
        {
            var (i, X, ti) = s;
            if (i == null)
                return jobs.Select(k =>
                    {
                        var tk = Math.Max(setup(i, k), k.ReleaseTime);
                        var Xk = new List<Job>().Append(k).Where(j => j.Deadline - j.ProcessTime >= tk + k.ProcessTime + setup(k, j));
                        var Tk = Math.Max(tk + k.ProcessTime - k.DueDate, 0);
                        return new ScheduleState(k, Xk, tk, k.Value, null);
                    });
            else
            {
                var F = RemainingCandidates(jobs, setup);
                return F(i, ti).Except(X).Select(k =>
                        {
                            var tk = Math.Max(ti + i.ProcessTime + setup(i, k), k.ReleaseTime);
                            var Xk = X.Append(k).Where(j => j.Deadline - j.ProcessTime >= tk + k.ProcessTime + setup(k, j));
                            var Tk = Math.Max(tk + k.ProcessTime - k.DueDate, 0);  // todo implement weighted tardiness
                            return new ScheduleState(k, Xk, tk, s.Value + k.Value, s);
                        }
                    );
            }
        }
        public static Func<Job, int, IEnumerable<Job>> RemainingCandidates(IEnumerable<Job> jobs, SetupTime setup) =>
            (Job i, int ti) => jobs.Where(j => ti + i.ProcessTime + setup(i, j) <= j.Deadline - j.ProcessTime);
        public static Dominator<ScheduleState> ScheduleStateDominator(SetupTime setup) =>
            (ScheduleState s1, ScheduleState s2) =>
            {
                ((Job i1, IEnumerable<Job> X1, int ti1), (Job i2, IEnumerable<Job> X2, int ti2)) = (s1, s2);
                return i1.Equals(i2)
                    && ti1 <= ti2
                    && s1.Value >= s2.Value
                    && X1.Subset(X2.Union(X1.Where((Job j) => j.Deadline - j.ProcessTime >= ti2 + i2.ProcessTime + setup(i2, j))));
            };
        
        public static bool Subset<T>(this IEnumerable<T> s1, IEnumerable<T> s2) =>
            !s1.Except(s2).Any();
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
        public void Deconstruct(out Job i, out IEnumerable<Job> X, out int ti)
        {
            i = LastScheduled;
            X = Exclude;
            ti = TimeScheduled;
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
