﻿using CSCore.Win32;
using System;
using System.Runtime.InteropServices;

namespace CSCore.XAudio2
{
    /// <summary>
    /// <see cref="XAudio2"/> is the class for the XAudio2 object that manages all audio engine states, the audio processing thread, the voice graph, and so forth.
    /// </summary>
    [Guid("60d8dac8-5aa1-4e8e-b597-2f5e2883d484")]
    public abstract class XAudio2 : ComObject
    {
        private const string N = "IXAudio2";

        public const int QuantumDenominator = 100;
        public const int MinimumSampleRate = 1000;
        public const int MaximumSampleRate = 200000;
        public const float MinFrequencyRatio = (1 / 1024.0f);
        public const float MaxFrequencyRatio = 1024.0f;
        public const float DefaultFrequencyRatio = 4.0f;
        public const int MaxAudioChannels = 64;
        public const int DefaultChannels = 0;
        public const int DefaultSampleRate = 0;

        public const int CommitAll = 0;
        public const int CommitNow = 0;

        /// <summary>
        /// Gets current resource usage details, such as available memory or CPU usage.
        /// </summary>
        public PerformanceData PerformanceData
        {
            get
            {
                PerformanceData performanceData;
                GetPerformanceDataNative(out performanceData);
                return performanceData;
            }
        }

        /// <summary>
        /// Internal default ctor.
        /// </summary>
        internal XAudio2()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XAudio2"/> class.
        /// </summary>
        /// <param name="ptr">Native pointer of the <see cref="XAudio2Voice"/> object.</param>
        protected XAudio2(IntPtr ptr)
            : base(ptr)
        {
        }

        /// <summary>
        /// Adds an <see cref="IXAudio2EngineCallback"/> from the <see cref="XAudio2"/> engine callback list.
        /// </summary>
        /// <param name="callback"><see cref="IXAudio2EngineCallback"/> object to add to the <see cref="XAudio2"/> engine callback list.</param>
        /// <returns>HRESULT</returns>
        public abstract int RegisterForCallbacksNative(IXAudio2EngineCallback callback);

        /// <summary>
        /// Adds an <see cref="IXAudio2EngineCallback"/> from the <see cref="XAudio2"/> engine callback list.
        /// </summary>
        /// <param name="callback"><see cref="IXAudio2EngineCallback"/> object to add to the <see cref="XAudio2"/> engine callback list.</param>
        public void RegisterForCallbacks(IXAudio2EngineCallback callback)
        {
            XAudio2Exception.Try(RegisterForCallbacksNative(callback), N, "RegisterForCallbacks");
        }

        /// <summary>
        /// Removes an <see cref="IXAudio2EngineCallback"/> from the <see cref="XAudio2"/> engine callback list.
        /// </summary>
        /// <param name="callback"><see cref="IXAudio2EngineCallback"/> object to remove from the <see cref="XAudio2"/> engine callback list. If the given interface is present more than once in the list, only the first instance in the list will be removed.</param>
        public abstract void UnregisterForCallbacks(IXAudio2EngineCallback callback);

        /// <summary>
        /// Creates and configures a source voice. For more information see http://msdn.microsoft.com/en-us/library/windows/desktop/microsoft.directx_sdk.ixaudio2.ixaudio2.createsourcevoice(v=vs.85).aspx.
        /// </summary>
        /// <param name="pSourceVoice">If successful, returns a pointer to the new <see cref="XAudio2SourceVoice"/> object.</param>
        /// <param name="sourceFormat">Pointer to a <see cref="WaveFormat"/>. The following formats are supported: 
        /// <ul><li>8-bit (unsigned) integer PCM</li><li>16-bit integer PCM (optimal format for XAudio2)</li><li>20-bit integer PCM (either in 24 or 32 bit containers)</li><li>24-bit integer PCM (either in 24 or 32 bit containers)</li><li>32-bit integer PCM</li><li>32-bit float PCM (preferred format after 16-bit integer)</li></ul>
        /// The number of channels in a source voice must be less than or equal to <see cref="MaxAudioChannels"/>. The sample rate of a source voice must be between <see cref="MinimumSampleRate"/> and <see cref="MaximumSampleRate"/>. 
        /// </param>
        /// <param name="flags"><see cref="VoiceFlags"/> that specify the behavior of the source voice. A flag can be <see cref="VoiceFlags.None"/> or a combination of one or more of the following. 
        /// Possible values are <see cref="VoiceFlags.NoPitch"/>, <see cref="VoiceFlags.NoSampleRateConversition"/> and <see cref="VoiceFlags.UseFilter"/>. <see cref="VoiceFlags.Music"/> is not supported on Windows.</param>
        /// <param name="maxFrequencyRatio">Highest allowable frequency ratio that can be set on this voice. The value for this argument must be between <see cref="MinFrequencyRatio"/> and <see cref="MaxFrequencyRatio"/>.</param>
        /// <param name="voiceCallback">Client-provided callback interface, <see cref="IXAudio2VoiceCallback"/>. This parameter is optional and can be null.</param>
        /// <param name="sendList">List of <see cref="VoiceSends"/> structures that describe the set of destination voices for the source voice. If <see cref="sendList"/> is NULL, the send list defaults to a single output to the first mastering voice created.</param>
        /// <param name="effectChain">List of <see cref="EffectChain"/> structures that describe an effect chain to use in the source voice. This parameter is optional and can be null.</param>
        /// <returns>HRESULT</returns>
        public abstract int CreateSourceVoiceNative(
            out IntPtr pSourceVoice,
            IntPtr sourceFormat,
            VoiceFlags flags,
            float maxFrequencyRatio,
            IXAudio2VoiceCallback voiceCallback,
            VoiceSends? sendList, //out
            EffectChain? effectChain
            );


        /// <summary>
        /// Creates and configures a source voice. For more information see http://msdn.microsoft.com/en-us/library/windows/desktop/microsoft.directx_sdk.ixaudio2.ixaudio2.createsourcevoice(v=vs.85).aspx.
        /// </summary>
        /// <param name="sourceFormat">Pointer to a <see cref="WaveFormat"/>. The following formats are supported: 
        /// <ul><li>8-bit (unsigned) integer PCM</li><li>16-bit integer PCM (optimal format for XAudio2)</li><li>20-bit integer PCM (either in 24 or 32 bit containers)</li><li>24-bit integer PCM (either in 24 or 32 bit containers)</li><li>32-bit integer PCM</li><li>32-bit float PCM (preferred format after 16-bit integer)</li></ul>
        /// The number of channels in a source voice must be less than or equal to <see cref="QuantumDenominator"/>. The sample rate of a source voice must be between <see cref="MinimumSampleRate"/> and <see cref="MaximumSampleRate"/>. 
        /// </param>
        /// <param name="flags"><see cref="VoiceFlags"/> that specify the behavior of the source voice. A flag can be <see cref="VoiceFlags.None"/> or a combination of one or more of the following. 
        /// Possible values are <see cref="VoiceFlags.NoPitch"/>, <see cref="VoiceFlags.NoSampleRateConversition"/> and <see cref="VoiceFlags.UseFilter"/>. <see cref="VoiceFlags.Music"/> is not supported on Windows.</param>
        /// <param name="maxFrequencyRatio">Highest allowable frequency ratio that can be set on this voice. The value for this argument must be between <see cref="MinFrequencyRatio"/> and <see cref="MaxFrequencyRatio"/>.</param>
        /// <param name="voiceCallback">Client-provided callback interface, <see cref="IXAudio2VoiceCallback"/>. This parameter is optional and can be null.</param>
        /// <param name="sendList">List of <see cref="VoiceSends"/> structures that describe the set of destination voices for the source voice. If <see cref="sendList"/> is NULL, the send list defaults to a single output to the first mastering voice created.</param>
        /// <param name="effectChain">List of <see cref="EffectChain"/> structures that describe an effect chain to use in the source voice. This parameter is optional and can be null.</param>
        /// <returns>If successful, returns a pointer to the new <see cref="XAudio2SourceVoice"/> object.</returns>
        public IntPtr CreateSourceVoicePtr(WaveFormat sourceFormat, VoiceFlags flags, float maxFrequencyRatio,
            IXAudio2VoiceCallback voiceCallback, VoiceSends? sendList, EffectChain? effectChain)
        {
            GCHandle hWaveFormat = GCHandle.Alloc(sourceFormat, GCHandleType.Pinned); //todo: do we really need to use GCHandle?
            try
            {
                IntPtr ptr;
                int result = CreateSourceVoiceNative(
                    out ptr,
                    hWaveFormat.AddrOfPinnedObject(),
                    flags,
                    maxFrequencyRatio,
                    voiceCallback,
                    sendList,
                    effectChain);
                XAudio2Exception.Try(result, N, "CreateSourceVoice");

                return ptr;
            }
            finally
            {
                if(hWaveFormat.IsAllocated)
                    hWaveFormat.Free();
            }
        }

        /// <summary>
        /// Creates and configures a source voice. For more information see http://msdn.microsoft.com/en-us/library/windows/desktop/microsoft.directx_sdk.ixaudio2.ixaudio2.createsourcevoice(v=vs.85).aspx.
        /// </summary>
        /// <param name="sourceFormat">Pointer to a <see cref="WaveFormat"/>. The following formats are supported: 
        /// <ul><li>8-bit (unsigned) integer PCM</li><li>16-bit integer PCM (optimal format for XAudio2)</li><li>20-bit integer PCM (either in 24 or 32 bit containers)</li><li>24-bit integer PCM (either in 24 or 32 bit containers)</li><li>32-bit integer PCM</li><li>32-bit float PCM (preferred format after 16-bit integer)</li></ul>
        /// The number of channels in a source voice must be less than or equal to <see cref="QuantumDenominator"/>. The sample rate of a source voice must be between <see cref="MinimumSampleRate"/> and <see cref="MaximumSampleRate"/>. 
        /// </param>
        /// <param name="flags"><see cref="VoiceFlags"/> that specify the behavior of the source voice. A flag can be <see cref="VoiceFlags.None"/> or a combination of one or more of the following. 
        /// Possible values are <see cref="VoiceFlags.NoPitch"/>, <see cref="VoiceFlags.NoSampleRateConversition"/> and <see cref="VoiceFlags.UseFilter"/>. <see cref="VoiceFlags.Music"/> is not supported on Windows.</param>
        /// <param name="maxFrequencyRatio">Highest allowable frequency ratio that can be set on this voice. The value for this argument must be between <see cref="MinFrequencyRatio"/> and <see cref="MaxFrequencyRatio"/>.</param>
        /// <param name="voiceCallback">Client-provided callback interface, <see cref="IXAudio2VoiceCallback"/>. This parameter is optional and can be null.</param>
        /// <param name="sendList">List of <see cref="VoiceSends"/> structures that describe the set of destination voices for the source voice. If <see cref="sendList"/> is NULL, the send list defaults to a single output to the first mastering voice created.</param>
        /// <param name="effectChain">List of <see cref="EffectChain"/> structures that describe an effect chain to use in the source voice. This parameter is optional and can be null.</param>
        /// <returns>If successful, returns a new <see cref="XAudio2SourceVoice"/> object.</returns>
        public XAudio2SourceVoice CreateSourceVoice(WaveFormat sourceFormat, VoiceFlags flags, float maxFrequencyRatio,
            IXAudio2VoiceCallback voiceCallback, VoiceSends? sendList, EffectChain? effectChain)
        {
            IntPtr ptr = CreateSourceVoicePtr(sourceFormat, flags, maxFrequencyRatio, voiceCallback, sendList, effectChain);
            return new XAudio2SourceVoice(ptr);
        }

        /// <summary>
        /// Creates and configures a source voice.
        /// </summary>
        /// <param name="sourceFormat">Pointer to a <see cref="WaveFormat"/>. The following formats are supported: 
        /// <ul><li>8-bit (unsigned) integer PCM</li><li>16-bit integer PCM (optimal format for XAudio2)</li><li>20-bit integer PCM (either in 24 or 32 bit containers)</li><li>24-bit integer PCM (either in 24 or 32 bit containers)</li><li>32-bit integer PCM</li><li>32-bit float PCM (preferred format after 16-bit integer)</li></ul>
        /// The number of channels in a source voice must be less than or equal to <see cref="QuantumDenominator"/>. The sample rate of a source voice must be between <see cref="MinimumSampleRate"/> and <see cref="MaximumSampleRate"/>. 
        /// </param>
        /// <param name="flags"><see cref="VoiceFlags"/> that specify the behavior of the source voice. A flag can be <see cref="VoiceFlags.None"/> or a combination of one or more of the following. 
        /// Possible values are <see cref="VoiceFlags.NoPitch"/>, <see cref="VoiceFlags.NoSampleRateConversition"/> and <see cref="VoiceFlags.UseFilter"/>. <see cref="VoiceFlags.Music"/> is not supported on Windows.</param>
        /// <returns>If successful, returns a new <see cref="XAudio2SourceVoice"/> object.</returns>
        public XAudio2SourceVoice CreateSourceVoice(WaveFormat sourceFormat, VoiceFlags flags)
        {
            const float defaultFreqRatio = 4.0f;
            return CreateSourceVoice(sourceFormat, flags, defaultFreqRatio, null, null, null);
        }

        /// <summary>
        /// Creates and configures a source voice.
        /// </summary>
        /// <param name="sourceFormat">Pointer to a <see cref="WaveFormat"/>. The following formats are supported: 
        /// <ul><li>8-bit (unsigned) integer PCM</li><li>16-bit integer PCM (optimal format for XAudio2)</li><li>20-bit integer PCM (either in 24 or 32 bit containers)</li><li>24-bit integer PCM (either in 24 or 32 bit containers)</li><li>32-bit integer PCM</li><li>32-bit float PCM (preferred format after 16-bit integer)</li></ul>
        /// The number of channels in a source voice must be less than or equal to <see cref="QuantumDenominator"/>. The sample rate of a source voice must be between <see cref="MinimumSampleRate"/> and <see cref="MaximumSampleRate"/>. 
        /// </param>
        /// <returns>If successful, returns a new <see cref="XAudio2SourceVoice"/> object.</returns>
        public XAudio2SourceVoice CreateSourceVoice(WaveFormat sourceFormat)
        {
            return CreateSourceVoice(sourceFormat, VoiceFlags.None);
        }

        /// <summary>
        /// Creates and configures a submix voice.
        /// </summary>
        /// <param name="pSubmixVoice">On success, returns a pointer to the new <see cref="XAudio2SubmixVoice"/> object.</param>
        /// <param name="inputChannels">Number of channels in the input audio data of the submix voice. The <see cref="inputChannels"/> must be less than or equal to <see cref="MaxAudioChannels"/>.</param>
        /// <param name="inputSampleRate">Sample rate of the input audio data of submix voice. This rate must be a multiple of <see cref="QuantumDenominator"/>. InputSampleRate must be between <see cref="MinimumSampleRate"/> and <see cref="MaximumSampleRate"/>.</param>
        /// <param name="flags">Flags that specify the behavior of the submix voice. It can be <see cref="VoiceFlags.None"/> or <see cref="VoiceFlags.UseFilter"/>.</param>
        /// <param name="processingStage">An arbitrary number that specifies when this voice is processed with respect to other submix voices, if the XAudio2 engine is running other submix voices. The voice is processed after all other voices that include a smaller <see cref="processingStage"/> value and before all other voices that include a larger <see cref="processingStage"/> value. Voices that include the same <see cref="processingStage"/> value are processed in any order. A submix voice cannot send to another submix voice with a lower or equal <see cref="processingStage"/> value. This prevents audio being lost due to a submix cycle.</param>
        /// <param name="sendList">List of <see cref="VoiceSends"/> structures that describe the set of destination voices for the submix voice. If <see cref="sendList"/> is NULL, the send list will default to a single output to the first mastering voice created.</param>
        /// <param name="effectChain">List of <see cref="EffectChain"/> structures that describe an effect chain to use in the submix voice. This parameter is optional and can be null.</param>
        /// <returns>HRESULT</returns>
        public abstract int CreateSubmixVoiceNative(out IntPtr pSubmixVoice, int inputChannels,
            int inputSampleRate, VoiceFlags flags,
            int processingStage, VoiceSends? sendList, EffectChain? effectChain);

        /// <summary>
        /// Creates and configures a submix voice.
        /// </summary>
        /// <param name="inputChannels">Number of channels in the input audio data of the submix voice. The <see cref="inputChannels"/> must be less than or equal to <see cref="MaxAudioChannels"/>.</param>
        /// <param name="inputSampleRate">Sample rate of the input audio data of submix voice. This rate must be a multiple of <see cref="QuantumDenominator"/>. InputSampleRate must be between <see cref="MinimumSampleRate"/> and <see cref="MaximumSampleRate"/>.</param>
        /// <param name="flags">Flags that specify the behavior of the submix voice. It can be <see cref="VoiceFlags.None"/> or <see cref="VoiceFlags.UseFilter"/>.</param>
        /// <param name="processingStage">An arbitrary number that specifies when this voice is processed with respect to other submix voices, if the XAudio2 engine is running other submix voices. The voice is processed after all other voices that include a smaller <see cref="processingStage"/> value and before all other voices that include a larger <see cref="processingStage"/> value. Voices that include the same <see cref="processingStage"/> value are processed in any order. A submix voice cannot send to another submix voice with a lower or equal <see cref="processingStage"/> value. This prevents audio being lost due to a submix cycle.</param>
        /// <param name="sendList">List of <see cref="VoiceSends"/> structures that describe the set of destination voices for the submix voice. If <see cref="sendList"/> is NULL, the send list will default to a single output to the first mastering voice created.</param>
        /// <param name="effectChain">List of <see cref="EffectChain"/> structures that describe an effect chain to use in the submix voice. This parameter is optional and can be null.</param>
        /// <returns>On success, returns a pointer to the new <see cref="XAudio2SubmixVoice"/> object.</returns>
        public IntPtr CreateSubmixVoicePtr(int inputChannels, int inputSampleRate, VoiceFlags flags,
            int processingStage, VoiceSends? sendList, EffectChain? effectChain)
        {
            IntPtr ptr;
            int result = CreateSubmixVoiceNative(out ptr, inputChannels, inputSampleRate, flags, processingStage, sendList,
                effectChain);
            XAudio2Exception.Try(result, N, "CreateSubmixVoiceNative");
            return ptr;
        }

        /// <summary>
        /// Creates and configures a submix voice.
        /// </summary>
        /// <param name="inputChannels">Number of channels in the input audio data of the submix voice. The <see cref="inputChannels"/> must be less than or equal to <see cref="MaxAudioChannels"/>.</param>
        /// <param name="inputSampleRate">Sample rate of the input audio data of submix voice. This rate must be a multiple of <see cref="QuantumDenominator"/>. InputSampleRate must be between <see cref="MinimumSampleRate"/> and <see cref="MaximumSampleRate"/>.</param>
        /// <param name="flags">Flags that specify the behavior of the submix voice. It can be <see cref="VoiceFlags.None"/> or <see cref="VoiceFlags.UseFilter"/>.</param>
        /// <param name="processingStage">An arbitrary number that specifies when this voice is processed with respect to other submix voices, if the XAudio2 engine is running other submix voices. The voice is processed after all other voices that include a smaller <see cref="processingStage"/> value and before all other voices that include a larger <see cref="processingStage"/> value. Voices that include the same <see cref="processingStage"/> value are processed in any order. A submix voice cannot send to another submix voice with a lower or equal <see cref="processingStage"/> value. This prevents audio being lost due to a submix cycle.</param>
        /// <param name="sendList">List of <see cref="VoiceSends"/> structures that describe the set of destination voices for the submix voice. If <see cref="sendList"/> is NULL, the send list will default to a single output to the first mastering voice created.</param>
        /// <param name="effectChain">List of <see cref="EffectChain"/> structures that describe an effect chain to use in the submix voice. This parameter is optional and can be null.</param>
        /// <returns>On success, returns a new <see cref="XAudio2SubmixVoice"/> object.</returns>
        public XAudio2SubmixVoice CreateSubmixVoice(int inputChannels, int inputSampleRate, VoiceFlags flags,
            int processingStage, VoiceSends? sendList, EffectChain? effectChain)
        {
            IntPtr ptr = CreateSubmixVoicePtr(inputChannels, inputSampleRate, flags, processingStage, sendList,
                effectChain);
            return new XAudio2SubmixVoice(ptr);
        }

        /// <summary>
        /// Creates and configures a submix voice.
        /// </summary>
        /// <param name="inputChannels">Number of channels in the input audio data of the submix voice. The <see cref="inputChannels"/> must be less than or equal to <see cref="MaxAudioChannels"/>.</param>
        /// <param name="inputSampleRate">Sample rate of the input audio data of submix voice. This rate must be a multiple of <see cref="QuantumDenominator"/>. InputSampleRate must be between <see cref="MinimumSampleRate"/> and <see cref="MaximumSampleRate"/>.</param>
        /// <param name="flags">Flags that specify the behavior of the submix voice. It can be <see cref="VoiceFlags.None"/> or <see cref="VoiceFlags.UseFilter"/>.</param>
        /// <returns>On success, returns a new <see cref="XAudio2SubmixVoice"/> object.</returns>
        public XAudio2SubmixVoice CreateSubmixVoice(int inputChannels, int inputSampleRate, VoiceFlags flags)
        {
            IntPtr ptr = CreateSubmixVoicePtr(inputChannels, inputSampleRate, flags, 0, null, null);
            return new XAudio2SubmixVoice(ptr);
        }

        /// <summary>
        /// Creates and configures a mastering voice.
        /// </summary>
        /// <param name="pMasteringVoice">If successful, returns a pointer to the new <see cref="XAudio2MasteringVoice"/> object.</param>
        /// <param name="inputChannels">Number of channels the mastering voice expects in its input audio. <see cref="inputChannels"/> must be less than or equal to <see cref="MaxAudioChannels"/>. 
        /// You can set InputChannels to <see cref="DefaultChannels"/>, which causes XAudio2 to try to detect the system speaker configuration setup.</param>
        /// <param name="inputSampleRate">Sample rate of the input audio data of the mastering voice. This rate must be a multiple of <see cref="QuantumDenominator"/>. <see cref="inputSampleRate"/> must be between <see cref="MinimumSampleRate"/> and <see cref="MaximumSampleRate"/>.
        /// You can set InputSampleRate to <see cref="DefaultSampleRate"/>, with the default being determined by the current platform.</param>
        /// <param name="flags">Flags that specify the behavior of the mastering voice. Must be 0.</param>
        /// <param name="device">Identifier of the device to receive the output audio. Specifying the default value of NULL causes XAudio2 to select the global default audio device.</param>
        /// <param name="effectChain"><see cref="EffectChain"/> structure that describes an effect chain to use in the mastering voice, or NULL to use no effects.</param>
        /// <param name="streamCategory">The audio stream category to use for this mastering voice.</param>
        /// <returns>HRESULT</returns>
        public abstract int CreateMasteringVoiceNative(out IntPtr pMasteringVoice, int inputChannels, int inputSampleRate,
            int flags,
            object device, EffectChain? effectChain, AudioStreamCategory streamCategory);

        /// <summary>
        /// Creates and configures a mastering voice.
        /// </summary>
        /// <param name="inputChannels">Number of channels the mastering voice expects in its input audio. <see cref="inputChannels"/> must be less than or equal to <see cref="MaxAudioChannels"/>. 
        /// You can set InputChannels to <see cref="DefaultChannels"/>, which causes XAudio2 to try to detect the system speaker configuration setup.</param>
        /// <param name="inputSampleRate">Sample rate of the input audio data of the mastering voice. This rate must be a multiple of <see cref="QuantumDenominator"/>. <see cref="inputSampleRate"/> must be between <see cref="MinimumSampleRate"/> and <see cref="MaximumSampleRate"/>.
        /// You can set InputSampleRate to <see cref="DefaultSampleRate"/>, with the default being determined by the current platform.</param>
        /// <param name="flags">Flags that specify the behavior of the mastering voice. Must be 0.</param>
        /// <param name="device">Identifier of the device to receive the output audio. Specifying the default value of NULL causes XAudio2 to select the global default audio device.</param>
        /// <param name="effectChain"><see cref="EffectChain"/> structure that describes an effect chain to use in the mastering voice, or NULL to use no effects.</param>
        /// <param name="streamCategory">The audio stream category to use for this mastering voice.</param>
        /// <returns>If successful, returns a pointer to the new <see cref="XAudio2MasteringVoice"/> object.</returns>
        public IntPtr CreateMasteringVoicePtr(int inputChannels, int inputSampleRate, int flags,
            object device, EffectChain? effectChain, AudioStreamCategory streamCategory)
        {
            IntPtr ptr;
            int result = CreateMasteringVoiceNative(out ptr, inputChannels, inputSampleRate, flags, device,
                effectChain, streamCategory);
            XAudio2Exception.Try(result, N, "CreateMasteringVoice");
            return ptr;
        }

        /// <summary>
        /// Creates and configures a mastering voice.
        /// </summary>
        /// <param name="inputChannels">Number of channels the mastering voice expects in its input audio. <see cref="inputChannels"/> must be less than or equal to <see cref="MaxAudioChannels"/>. 
        /// You can set InputChannels to <see cref="DefaultChannels"/>, which causes XAudio2 to try to detect the system speaker configuration setup.</param>
        /// <param name="inputSampleRate">Sample rate of the input audio data of the mastering voice. This rate must be a multiple of <see cref="QuantumDenominator"/>. <see cref="inputSampleRate"/> must be between <see cref="MinimumSampleRate"/> and <see cref="MaximumSampleRate"/>.
        /// You can set InputSampleRate to <see cref="DefaultSampleRate"/>, with the default being determined by the current platform.</param>
        /// <param name="device">Identifier of the device to receive the output audio. Specifying the default value of NULL causes XAudio2 to select the global default audio device.</param>
        /// <param name="effectChain"><see cref="EffectChain"/> structure that describes an effect chain to use in the mastering voice, or NULL to use no effects.</param>
        /// <param name="streamCategory">The audio stream category to use for this mastering voice.</param>
        /// <returns>If successful, returns a new <see cref="XAudio2MasteringVoice"/> object.</returns>
        public XAudio2MasteringVoice CreateMasteringVoice(int inputChannels, int inputSampleRate,
            object device, EffectChain? effectChain, AudioStreamCategory streamCategory)
        {
            return new XAudio2MasteringVoice(CreateMasteringVoicePtr(inputChannels, inputSampleRate, 0, device, effectChain, streamCategory));
        }

        /// <summary>
        /// Creates and configures a mastering voice.
        /// </summary>
        /// <param name="inputChannels">Number of channels the mastering voice expects in its input audio. <see cref="inputChannels"/> must be less than or equal to <see cref="MaxAudioChannels"/>. 
        /// You can set InputChannels to <see cref="DefaultChannels"/>, which causes XAudio2 to try to detect the system speaker configuration setup.</param>
        /// <param name="inputSampleRate">Sample rate of the input audio data of the mastering voice. This rate must be a multiple of <see cref="QuantumDenominator"/>. <see cref="inputSampleRate"/> must be between <see cref="MinimumSampleRate"/> and <see cref="MaximumSampleRate"/>.
        /// You can set InputSampleRate to <see cref="DefaultSampleRate"/>, with the default being determined by the current platform.</param>
        /// <param name="device">Identifier of the device to receive the output audio. Specifying the default value of NULL causes XAudio2 to select the global default audio device.</param>
        /// <returns>If successful, returns a new <see cref="XAudio2MasteringVoice"/> object.</returns>
        public XAudio2MasteringVoice CreateMasteringVoice(int inputChannels, int inputSampleRate, 
            object device)
        {
            return new XAudio2MasteringVoice(CreateMasteringVoicePtr(inputChannels, inputSampleRate, 0, device, null, AudioStreamCategory.GameEffects));
        }

        /// <summary>
        /// Creates and configures a mastering voice.
        /// </summary>
        /// <param name="inputChannels">Number of channels the mastering voice expects in its input audio. <see cref="inputChannels"/> must be less than or equal to <see cref="MaxAudioChannels"/>. 
        /// You can set InputChannels to <see cref="DefaultChannels"/>, which causes XAudio2 to try to detect the system speaker configuration setup.</param>
        /// <param name="inputSampleRate">Sample rate of the input audio data of the mastering voice. This rate must be a multiple of <see cref="QuantumDenominator"/>. <see cref="inputSampleRate"/> must be between <see cref="MinimumSampleRate"/> and <see cref="MaximumSampleRate"/>.
        /// You can set InputSampleRate to <see cref="DefaultSampleRate"/>, with the default being determined by the current platform.</param>
        /// <returns>If successful, returns a new <see cref="XAudio2MasteringVoice"/> object.</returns>
        public XAudio2MasteringVoice CreateMasteringVoice(int inputChannels, int inputSampleRate)
        {
            return new XAudio2MasteringVoice(CreateMasteringVoicePtr(inputChannels, inputSampleRate, 0, GetDefaultDevice(), null, AudioStreamCategory.GameEffects));
        }

        /// <summary>
        /// Creates and configures a mastering voice.
        /// </summary>
        /// <returns>If successful, returns a new <see cref="XAudio2MasteringVoice"/> object.</returns>
        public XAudio2MasteringVoice CreateMasteringVoice()
        {
            return CreateMasteringVoice(DefaultChannels, DefaultSampleRate);
        }

        /// <summary>
        /// Starts the audio processing thread.
        /// </summary>
        /// <returns>HRESULT</returns>
        public abstract int StartEngineNative();

        /// <summary>
        /// Starts the audio processing thread.
        /// </summary>
        public void StartEngine()
        {
            XAudio2Exception.Try(StartEngineNative(), N, "StartEngine");
        }

        /// <summary>
        /// Stops the audio processing thread.
        /// </summary>
        public abstract void StopEngine();

        /// <summary>
        /// Atomically applies a set of operations that are tagged with a given identifier.
        /// </summary>
        /// <param name="operationSet">Identifier of the set of operations to be applied. To commit all pending operations, pass <see cref="CommitAll"/>.</param>
        /// <returns>HRESULT</returns>
        public abstract int CommitChangesNative(int operationSet);

        /// <summary>
        /// Atomically applies a set of operations that are tagged with a given identifier.
        /// </summary>
        /// <param name="operationSet">Identifier of the set of operations to be applied. To commit all pending operations, pass <see cref="CommitAll"/>.</param>
        public void CommitChanges(int operationSet)
        {
            XAudio2Exception.Try(CommitChangesNative(operationSet), N, "CommitChanges");
        }

        /// <summary>
        /// Atomically applies a set of operations that are tagged with a given identifier.
        /// </summary>
        public void CommitChanges()
        {
            CommitChanges(CommitAll);
        }

        /// <summary>
        /// Returns current resource usage details, such as available memory or CPU usage.
        /// </summary>
        /// <param name="performanceData">On success, pointer to an <see cref="CSCore.XAudio2.PerformanceData"/> structure that is returned.</param>
        /// <returns>HRESULT</returns>
        public abstract void GetPerformanceDataNative(out PerformanceData performanceData);

        /// <summary>
        /// Changes <b>global</b> debug logging options for XAudio2.
        /// </summary>
        /// <param name="debugConfiguration"><see cref="DebugConfiguration"/> structure that contains the new debug configuration.</param>
        /// <param name="reserved">Reserved parameter. Must me NULL.</param>
        /// <returns>HRESULT</returns>
        public abstract void SetDebugConfigurationNative(DebugConfiguration debugConfiguration, IntPtr reserved);

        /// <summary>
        /// Changes <b>global</b> debug logging options for XAudio2.
        /// </summary>
        /// <param name="debugConfiguration"><see cref="DebugConfiguration"/> structure that contains the new debug configuration.</param>
        public void SetDebugConfiguration(DebugConfiguration debugConfiguration)
        {
            SetDebugConfigurationNative(debugConfiguration, IntPtr.Zero);
        }

        protected abstract object GetDefaultDevice();
    }
}