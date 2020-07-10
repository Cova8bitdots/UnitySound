using UnityEngine.Audio;

namespace CovaTech.UnitySound
{
    /// <summary>
    /// AudioMixerのエフェクトパラメータ設定変更用API
    /// </summary>
    public interface IMixerEffectController
    {
        /// <summary>
        /// LPF のカットオフ周波数設定
        /// </summary>
        /// <param name="_category">対象カテゴリ</param>
        /// <param name="normalizedFreq">正規化周波数</param>
        void SetLowPassFilter(SOUND_CATEGORY _category, float normalizedFreq );

        /// <summary>
        /// HPF のカットオフ周波数設定
        /// </summary>
        /// <param name="_category">対象カテゴリ</param>
        /// <param name="normalizedFreq">正規化周波数</param>
        void SetHighPassFilter(SOUND_CATEGORY _category,  float normalizedFreq );
    }
}