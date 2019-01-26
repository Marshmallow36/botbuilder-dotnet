﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.Dialogs
{
    public class PromptOptions
    {
        /// <summary>
        /// The value for the prompt to work with.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the initial prompt to send the user as <seealso cref="Activity"/>Activity.
        /// </summary>
        /// <value>
        /// The initial prompt to send the user as <seealso cref="Activity"/>Activity.
        /// </value>
        public Activity Prompt { get; set; }

        /// <summary>
        /// Gets or sets the retry prompt to send the user as <seealso cref="Activity"/>Activity.
        /// </summary>
        /// <value>
        /// The retry prompt to send the user as <seealso cref="Activity"/>Activity.
        /// </value>
        public Activity RetryPrompt { get; set; }
    }
}