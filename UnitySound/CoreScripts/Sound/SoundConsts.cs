using UnityEngine;
using UnityEngine.Audio;
namespace CovaTech.UnitySound
{
    /// <summary>
    /// サウンド関連の定数管理クラス
    /// </summary>
    public static class SoundConsts
    {
        //------------------------------------------------------------------
        // 全般
        //------------------------------------------------------------------
        #region  ===== GENERAL =====
        public const int INVALID_HANDLER = -1;

        // デフォルトの音量[deg]
        public const float DEFAULT_VOLUME = 1.0f;

        #endregion //) ===== GENERAL =====
        
        
    }

    /// <summary>
    /// オーディオのカテゴリ
    /// </summary>
    public enum SOUND_CATEGORY
    {
        UNDEFINED = -1,

        /* 最終的な出力*/
        MASTER,

        /* BGM 系列 */
        BGM_MASTER,
        SYSTEM_BGM,
        DIEGETIC_BGM,


        /* SE 系列 */
        SE_MASTER,
        SYSTEM_SE,
        JINGLE,
        ENVIROMENT_SE,
        DIEGETIC_SE,

        /* VOICE 系列 */
        VOICE_MASTER,
        SYSTEM_VOICE,
    }

    /// <summary>
    /// サウンドアイテムのの状態
    /// </summary>
    public enum SOUND_STATE
    {
        UNDEFINED,          // 未定義State
        INIT,               // 初期化中
        IDLE,               // 待機中
        PRE_PLAY,           // 再生準備中
        PLAYING,            // 再生中
        PRE_STOP,           // 停止準備中
        STOP,               // 停止中
    }
}
