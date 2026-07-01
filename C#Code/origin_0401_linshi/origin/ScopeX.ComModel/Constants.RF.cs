using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.ComModel
{
    public static partial class Constants
    {

        public const Int64 RF_SAMPLING_FREQUENCY = 20_000_000_000;

        public const Int64 RF_FREQUENCY_MIN = 0;
        public const Int64 RF_FREQUENCY_MAX = 20_000_000_000;

        public const Int32 RF_ACQUIRE_LENGTH = 10_000;
        
        public const Int32 RF_FFT_LENGTH = 1024;
        public const Int32 RF_FFT_LENGTH_MIN = 64;
        public const Int32 RF_FFT_LENGTH_MAX = 16384;

        public const Int32 RF_HSCALE_MIN = 100;
        public const Int32 RF_HSCALE_MAX = 1_000_000_000;

        public const Int32 RF_FREQUENCY_SCALE_MAX = 1_000_000_000;
        public const Int32 RF_FREQUENCY_SCALE_MIN = 1; 
        public const Int32 RF_FREQUENCY_SCALE = 10;

        public const Int32 RF_AVERAGE_TIMES = 10;
        public const Int32 RF_AVERAGE_TIMES_MIN = 1;
        public const Int32 RF_AVERAGE_TIMES_MAX = 10_000;

        public const Int64 RF_SPAN_MIN = 1_000;
        public const Int64 RF_SPAN_MAX = 20_000_000_000;

        public const Int64 RF_RBW_MIN = 10;
        public const Int64 RF_RBW_MAX = 2_000_000;

        public const Int32 RF_FFT_POWER_MIN = 6;
        public const Int32 RF_FFT_POWER_MAX = 14;

        public const Int64 RF_START_FREQUENCY_MIN = RF_FREQUENCY_MIN;
        public const Int64 RF_START_FREQUENCY_MAX = RF_FREQUENCY_MAX - RF_SPAN_MIN;

        public const Int64 RF_END_FREQUENCY_MIN = RF_FREQUENCY_MIN + RF_SPAN_MIN;
        public const Int64 RF_END_FREQUENCY_MAX = RF_FREQUENCY_MAX;

        public const Int64 RF_CENTER_FREQUENCY_MIN = RF_FREQUENCY_MIN + RF_SPAN_MIN / 2;
        public const Int64 RF_CENTER_FREQUENCY_MAX = RF_FREQUENCY_MAX - RF_SPAN_MIN / 2;

        public const Int64 RF_FREQUENCY_RANGE = 8_000_000_000;

        public const Int64 RF_ADC_ACQ_VOLTAGE_UV_MAX = 400_000;//0 - 0.8V
        public const Int64 RF_ADC_ACQ_VOLTAGE_UV_MIN = -400_000;

        public const Int64 RF_MAX_IQ_BW = 1_600_000_000;

        public const Int64 RF_REF_LEVEL_VALUE = 5;
        public const Int64 RF_REF_LEVEL_STEP = 3;

        public const Int64 RF_AMP_SCALE = 15;
        public const Double RF_AMP_MIN_SCALE = 0.1;
        public const Double RF_AMP_MAX_SCALE = 50;
        public const Int64 RF_FIGURE_START_AMPLITUDE = 15;
        public const Int64 RF_FIGURE_END_AMPLITUDE = -135;
        public const Int64 RF_FIGURE_CENTER_AMPLITUDE = -60;
        public const Int64 RF_CENTER_AMPLITUDE_MIN = -500;
        public const Int64 RF_CENTER_AMPLITUDE_MAX = 500;

        public const Int64 RF_REF_LEVEL = 5;
        public const Int64 RF_REF_LEVEL_MIN = -100;
        public const Int64 RF_REF_LEVEL_MAX = 100;

        public const Int32 RF_THRESHOLD = -50;
        public const Int32 RF_THRESHOLD_MAX = 100;
        public const Int32 RF_THRESHOLD_MIN = -200;
        public const Int32 RF_MAX_MARKER_COUNT_MAX = 11;
        public const Int32 RF_MAX_MARKER_COUNT_MIN = 1;
        public const Int32 RF_EXCURSION = 10;
        public const Int32 RF_EXCURSION_MAX = RF_THRESHOLD_MAX - RF_THRESHOLD_MIN - 1;
        public const Int32 RF_EXCURSION_MIN = 0;

        //public const Double RF_SPAN = 1_000_000;

        //public const Double RF_RBW = 10_000;

        public const Int32 RF_FIGURE_START_PHASE = 60;
        public const Int32 RF_FIGURE_CENTER_PHASE = 0;
        public const Int32 RF_FIGURE_END_PHASE = -60;
        public const Int32 RF_PHASE_SCALE = 60;
        public const Int32 RF_PHASE_SCALE_MIN = 1;
        public const Int32 RF_PHASE_SCALE_MAX = 360;

        public const Int64 RF_TIME_SCALE_US = 1000;
        public const Int64 RF_TIME_MIN_SCALE_PS = 1;
        public const Int64 RF_TIME_MAX_SCALE_PS = 10_000_000_000_000;
        public const Int64 RF_FIGURE_START_TIME_V = 500_000;
        public const Int64 RF_FIGURE_CENTER_TIME_V = 0;
        public const Int64 RF_FIGURE_END_TIME_V = -500_000;

        public const Int64 RF_FIGURE_START_TIME_H = 0;
        public const Int64 RF_FIGURE_CENTER_TIME_H = 500_000;
        public const Int64 RF_FIGURE_END_TIME_H = 1_000_000;

        public const Int64 RF_TRANSLATE_SAMPLERATE = 20_000_000_000;

        public const Int32 RF_SPECTRUGRAM_HEIGHT = 300;

        public const Int32 RF_STFT_DATA_LENGTH = 1024;
        public const Int32 RF_STFT_DATA_LENGTH_MIN = 64;
        public const Int32 RF_STFT_DATA_LENGTH_MAX = 16384;
        public const Int32 RF_STFT_STEP = 1024;
        public const Int32 RF_STFT_STEP_MIN = 2;
        public const Int32 RF_STFT_STEP_MAX = RF_STFT_DATA_LENGTH_MAX - RF_FFT_LENGTH_MIN;



        public const Double RF_MEASURE_MIN_THD_CHANNEL_SPAN = 4_000_000_000;
        public const Double RF_MEASURE_MAX_THD_CHANNEL_SPAN = 100_000_000_000_000;
        public const Double RF_MEASURE_MIN_THD_CHANNEL_SPACING = 5_000_000_000;
        public const Double RF_MEASURE_MAX_THD_CHANNEL_SPACING = 100_000_000_000_000;
        public const Int32 RF_MEASURE_MIN_THD_CHANNEL_COUNT = 3;
        public const Int32 RF_MEASURE_MAX_THD_CHANNEL_COUNT = 5;

        public const Double RF_MEASURE_MIN_ACPR_CHANNEL_SPAN = 4_000_000_000;
        public const Double RF_MEASURE_MAX_ACPR_CHANNEL_SPAN = 100_000_000_000_000;
        public const Double RF_MEASURE_MIN_ACPR_CHANNEL_SPACING = 5_000_000_000;
        public const Double RF_MEASURE_MAX_ACPR_CHANNEL_SPACING = 100_000_000_000_000;
        public const Int32 RF_MEASURE_MIN_ACPR_CHANNEL_COUNT = 1;
        public const Int32 RF_MEASURE_MAX_ACPR_CHANNEL_COUNT = 5;

        public const Double RF_MEASURE_MIN_OB_PERCENTAGE = 0;
        public const Double RF_MEASURE_DEFAULT_OB_PERCENTAGE = 99;
        public const Double RF_MEASURE_MAX_OB_PERCENTAGE = 100;
        public const Int32 RF_MEASURE_MIN_OB_DB_DOWN = -1000;
        public const Int32 RF_MEASURE_MAX_OB_DB_DOWN = -1;
        public const Double RF_MEASURE_MIN_OB_CHANNEL_SPAN = 4_000_000_000;
        public const Double RF_MEASURE_MAX_OB_CHANNEL_SPAN = 100_000_000_000_000;

        public const Double RF_MEASURE_MIN_CP_CHANNEL_SPAN = 5_000_000_000;
        public const Double RF_MEASURE_MAX_CP_CHANNEL_SPAN = 100_000_000_000_000;
    }
}
