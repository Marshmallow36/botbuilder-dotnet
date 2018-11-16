﻿using System.Collections.Generic;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.NumberWithUnit;

namespace Microsoft.Bot.Builder.ComposableDialogs.Recognizers
{
    public class TemperatureEntityRecognizer : BaseEntityRecognizer
    {
        public TemperatureEntityRecognizer() { }

        protected override List<ModelResult> Recognize(string text, string culture)
        {
            return NumberWithUnitRecognizer.RecognizeTemperature(text, culture);
        }
    }
}
