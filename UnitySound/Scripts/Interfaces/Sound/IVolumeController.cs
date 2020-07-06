using Cysharp.Threading.Tasks;

namespace CovaTech.UnitySound
{
    public interface IVolumeController
    {
        /// <summary>
        /// 指定カテゴリの音量を更新
        /// </summary>
        /// <param name="_category"></param>
        /// <param name="_volume"></param>
        void UpdateVolume( SOUND_CATEGORY _category, float _volume);

        /// <summary>
        /// 指定オーディオの音量を返す
        /// </summary>
        /// <param name="_category"></param>
        /// <returns></returns>
        float GetVolume( SOUND_CATEGORY _category) ;
    }
}
