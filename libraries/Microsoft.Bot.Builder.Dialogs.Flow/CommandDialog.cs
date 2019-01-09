﻿using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Builder.Dialogs.Flow
{
    /// <summary>
    /// CommandDialog - Call a IDialog and then execute Command when completed
    /// </summary>
    public class CommandDialog : Dialog, IDialog
    {
        public CommandDialog(string dialogId = null) : base(dialogId)
        {
        }

        /// <summary>
        /// Innner dialog to call
        /// </summary>
        public string DialogId { get; set; }

        /// <summary>
        /// Command to perform when dialog is completed
        /// </summary>
        public IDialogCommand Command { get; set; }

        /// <summary>
        /// When this dialog is started we start the inner dialog
        /// </summary>
        /// <param name="dialogContext"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dialogContext, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var state = dialogContext.ActiveDialog?.State;
            options = MergeDefaultOptions(options);
            state[$"{this.Id}.options"] = options;
            state[$"{this.Id}.CommandCalled"] = false;
            return await BeginInnerDialog(dialogContext, options, cancellationToken);
        }

        private object MergeDefaultOptions(object options)
        {
            dynamic opts;
            if (options != null && DefaultOptions != null)
            {
                opts = JObject.FromObject(options);
                opts.Merge(this.DefaultOptions);
            }
            else
            {
                opts = options ?? this.DefaultOptions;
            }

            options = options ?? this.DefaultOptions;
            return options;
        }

        /// <summary>
        /// The only time we continue is when a ContinueDialogAction is executed, in which case we start the inner dialog again (since it ended already)
        /// </summary>
        /// <param name="dialogContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<DialogTurnResult> ContinueDialogAsync(DialogContext dialogContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var state = dialogContext.ActiveDialog.State;
            dynamic options = state[$"{this.Id}.options"];
            state[$"{this.Id}.CommandCalled"] = false;
            return await BeginInnerDialog(dialogContext, options, cancellationToken);
        }

        /// <summary>
        /// Inner dialog has completed
        /// </summary>
        /// <param name="dialogContext"></param>
        /// <param name="reason"></param>
        /// <param name="result"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<DialogTurnResult> ResumeDialogAsync(DialogContext dialogContext, DialogReason reason, object result = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var state = dialogContext.ActiveDialog.State;
            var options = state[$"{this.Id}.options"];
            var dialogTurnResult = new DialogTurnResult(DialogTurnStatus.Complete, result);
            switch (reason)
            {
                case DialogReason.EndCalled:
                    // call the OnCommand handler
                    bool CommandCalled = (bool)state[$"{this.Id}.CommandCalled"];
                    if (this.Command != null && !CommandCalled)
                    {
                        state[$"{this.Id}.CommandCalled"] = true;
                        state["DialogTurnResult"] = dialogTurnResult;
                        return await this.Command.Execute(dialogContext, options, dialogTurnResult, cancellationToken);
                    }
                    break;
            }

            // no routing, return the result as our result 
            state.Remove($"{this.Id}.CommandCalled");
            return await dialogContext.EndDialogAsync(result, cancellationToken);
        }


        private async Task<DialogTurnResult> BeginInnerDialog(DialogContext dialogContext, object options, CancellationToken cancellationToken)
        {
            var state = dialogContext.ActiveDialog.State;

            if (!string.IsNullOrEmpty(this.DialogId))
            {
                // start the inner dialog
                return await dialogContext.BeginDialogAsync(this.DialogId, options, cancellationToken);
            }

            // call the OnCompleted handler
            var dialogTurnResult = new DialogTurnResult(DialogTurnStatus.Complete);

            if (this.Command != null)
            {
                state[$"{this.Id}.CommandCalled"] = true;
                state["DialogTurnResult"] = dialogTurnResult;
                return await this.Command.Execute(dialogContext, options, dialogTurnResult, cancellationToken);
            }

            // no routing, return the result as our result 
            return dialogTurnResult;
        }

    }
}
