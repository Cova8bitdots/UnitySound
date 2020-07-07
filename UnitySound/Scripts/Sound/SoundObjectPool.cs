using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using CovaTech.Lib;

namespace CovaTech.UnitySound
{

    public class SoundObjectPool : CovaTech.Lib.ObjectPool<SoundItem>
    {

        //-----------------------------------------------------
        // メンバ変数
        //-----------------------------------------------------
        #region ===== MEMBER_VARIABLES =====
        SoundItem m_itemPrefab = null;

        private Transform m_parentObject = null;

        #endregion //) ===== MEMBER_VARIABLES =====


        //-----------------------------------------------------
        // 初期化
        //-----------------------------------------------------
        #region ===== INITIALIZE =====
        public void Init( SoundItem _itemPrefab, Transform _parent, string _itemName, int _poolSize = DEFAULT_POOL_COUNT)
        {
            Debug.Assert(_itemPrefab != null);
            Debug.Assert(_parent != null);
            m_itemPrefab = _itemPrefab;
            m_parentObject = _parent;
            InitializePool(_itemName, _poolSize);
        }

        #endregion //) ===== INITIALIZE =====




        //-----------------------------------------------------
        // 提供メソッド
        //-----------------------------------------------------
        #region ===== IObjectPool =====
        /// <summary>
        /// Item生成
        /// </summary>
        /// <returns></returns>
        public override SoundItem CreateItem()
        {
            if( m_itemPrefab == null || m_parentObject == null)
            {
                return null;
            }
            var item = UnityEngine.Object.Instantiate(m_itemPrefab, m_parentObject);
            item?.Init(CurrentPoolCount + 1, this);
            return item;
        }

        /// <summary>
        /// 拝借前の設定
        /// </summary>
        /// <param name="_item">Pool からの貸し出し対象</param>
        protected override void OnBeforeRent(SoundItem _item)
        {
            _item.SetEnable();
        }


        /// <summary>
        /// Item 返却前処理
        /// </summary>
        protected override void OnBeforeReturn(SoundItem _item)
        {

            base.OnBeforeReturn(_item);
        }

        #endregion //) ===== IObjectPool =====

        /// <summary>
        /// 指定カテゴリで現在再生している数を返す
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public int GetCurrentPlayItemCount( SOUND_CATEGORY _type)
        {
            int c = 0;
            for (int i = 0; i < m_allItems.Count; i++)
            {
                var item = m_allItems[i];
                if( item.AudioType == _type && item.IsUsed() )
                {
                    c++;
                }
            }
            return c;
        }

        /// <summary>
        /// 指定AudioTypeのItemをすべて返す
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public List<SoundItem> GetItems(SOUND_CATEGORY _type)
        {
            List<SoundItem> list = new List<SoundItem>();
            foreach( var item in m_allItems)
            {
                if( !item.IsUsed() || item.AudioType != _type)
                {
                    continue;
                }
                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// 現状のQueueを掃除
        /// </summary>
        public void Clear()
        {
            if( m_allItems == null || m_allItems.Count < 1)
            {
                return;
            }

            for (int i = 0; i < m_allItems.Count; i++)
            {
                m_allItems[i]?.Dispose();
            }
        }
    }
}
