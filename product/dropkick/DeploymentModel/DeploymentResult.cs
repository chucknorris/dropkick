// Copyright 2007-2010 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
using System.Diagnostics;
using System.Text;

namespace dropkick.DeploymentModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    [DebuggerDisplay("{ResultsToList()}")]
    public class DeploymentResult : IEnumerable<DeploymentItem>
    {
        readonly List<DeploymentItem> _items = new List<DeploymentItem>();

        public IList<DeploymentItem> Results
        {
            get { return new ReadOnlyCollection<DeploymentItem>(_items); }
        }

        public int ResultCount
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// Deployment encountered a fatal error, so it should stop right now...
        /// </summary>
        public bool ShouldAbort { get; private set; }

        /// <summary>
        /// Sets 'ShouldAbort' to true, and logs an Error Item.
        /// </summary>
        /// <param name="abortReason"></param>
        public void AddAbort(string abortReason) {
           ShouldAbort = true;
           AddItem(DeploymentItemStatus.Error, "Aborting deployment! Reason: {0}".FormatWith(abortReason));
        }

        #region IEnumerable<DeploymentItem> Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<DeploymentItem> GetEnumerator()
        {
            foreach (var item in _items)
            {
                yield return item;
            }
            yield break;
        }

        #endregion

        public void AddGood(string message)
        {
            AddItem(DeploymentItemStatus.Good, message);
        }

        public void AddGood(string messageFormat, params object[] args)
        {
            AddItem(DeploymentItemStatus.Good, string.Format(messageFormat, args));
        }

        public void AddAlert(string message)
        {
            AddItem(DeploymentItemStatus.Alert, message);
        }

        public void AddAlert(string messageFormat, params object[] args)
        {
            AddItem(DeploymentItemStatus.Alert, string.Format(messageFormat, args));
        }

        public void AddVerbose(string messageFormat, params object[] args)
        {
            AddItem(DeploymentItemStatus.Verbose, string.Format(messageFormat, args));
        }

        public void AddNote(string message)
        {
            AddItem(DeploymentItemStatus.Note, message);
        }

        public void AddNote(string messageFormat, params object[] args)
        {
            AddItem(DeploymentItemStatus.Note, string.Format(messageFormat, args));
        }

        public void AddError(string message)
        {
            AddItem(DeploymentItemStatus.Error, message);
        }

        public void AddError(string message, Exception exception)
        {
            AddItem(DeploymentItemStatus.Error, "{0}:{1}".FormatWith(message,exception));
        }

        void AddItem(DeploymentItemStatus status, string message)
        {
            _items.Add(new DeploymentItem(status, message));
        }

        public void Add(DeploymentItem item)
        {
            _items.Add(item);
        }

        public DeploymentResult MergedWith(DeploymentResult result)
        {
            foreach (var item in result.Results)
            {
                _items.Add(item);
            }
            ShouldAbort |= result.ShouldAbort;

            return this;
        }

        public bool ContainsError()
        {
            return _items.Any(x => x.Status == DeploymentItemStatus.Error);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var item in _items)
            {

                sb.AppendFormat("{0}{1}",item.Message ?? "{null}" , Environment.NewLine);
            }

            return sb.ToString();
        }

        public string ResultsToList()
        {
            var sb = new StringBuilder();
            sb.Append("|");
            foreach (var item in _items)
            {
                sb.AppendFormat("{0}{1}", item.Message ?? "{null}", "|");
            }

            return sb.ToString();
        }
    }
}