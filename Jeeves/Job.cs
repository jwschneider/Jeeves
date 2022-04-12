using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace Jeeves
{
    public class Job
    {
        public string Identity { get; set; }
        public int Value { get; set; }
        public int ReleaseTime { get; set; }
        public int ProcessTime { get; set; }
        public int DueDate { get; set; }
        public int Deadline { get; set; }

        public Job()
        {

        }

        public Job(string identity, int value, int releaseTime, int processTime, int dueDate, int deadline)
        {
            Identity = identity;
            Value = value;
            ReleaseTime = releaseTime;
            ProcessTime = processTime;
            DueDate = dueDate;
            Deadline = deadline;
        }
        public static Job CreateJobFromArray(int[] array)
        {
            if (array.GetLength(0) != 6)
            {
                throw new ArgumentException($"Rows of input array are of incorrect length: {array.GetLength}");
            }
            return new Job(array[0].ToString(), array[1], array[2], array[3], array[4], array[5]);

        }
        // Array format for job is 6 columns: identity, value, releaseTime, processTime, dueDate, deadline
        public static List<Job> CreateJobsFromMatrix(int[,] matrix)
        {
            if (matrix.GetLength(1) != 6)
            {
                throw new ArgumentException($"Rows of input matrix are of incorrect length {matrix.GetLength(1)}");
            }
            List<Job> jobs = new List<Job>();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                jobs.Add(new Job(matrix[i, 0].ToString(), matrix[i, 1], matrix[i, 2], matrix[i, 3], matrix[i, 4], matrix[i, 5]));
            }
            return jobs;
        }
        // two jobs are identical if they have the same identity
        public bool IdenticalTo(object obj) =>
            (obj is Job) ?
                String.Equals(((Job)obj).Identity, Identity) :
                false;

        // two jobs are equal if they have the same properties, regardless of identity
        public override bool Equals(object obj) =>
            (obj is Job) ?
                DateTime.Equals(((Job)obj).ReleaseTime, ReleaseTime) &&
                    TimeSpan.Equals(((Job)obj).ProcessTime, ProcessTime) &&
                    DateTime.Equals(((Job)obj).DueDate, DueDate) &&
                    DateTime.Equals(((Job)obj).Deadline, Deadline) &&
                    ((Job)obj).Value == Value :
                false;

        public bool Equals(Job other, out string message)
        {
            string[] jobProperties = new string[] { "Identity", "ReleaseTime", "ProcessTime", "DueDate", "Deadline", "Value" };
            var unequalProperty = jobProperties.Select<string, (bool, string)>(
                property =>
                    {
                        var thisProp = this.GetType().GetProperty(property).GetValue(this);
                        var otherProp = other.GetType().GetProperty(property).GetValue(other);
                        return (!Equals(thisProp, otherProp), $"this {property}: {thisProp}, other {property}: {otherProp}");
                    })
                .Where(tuple => tuple.Item1)
                .FirstOrDefault();
            message = unequalProperty.Item2;
            return !unequalProperty.Item1;
        }
        public override string ToString()
        {
            return $"{{Identity:{Identity}, ReleaseTime: {ReleaseTime}, ProcessTime: {ProcessTime}, DueDate: {DueDate}, Deadline: {Deadline}, Value: {Value}}}";
        }
    }
}
