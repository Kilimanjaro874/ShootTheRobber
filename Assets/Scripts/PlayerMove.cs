using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerMove : MonoBehaviour
{
    /// <summary>
    /// キャラクターが持つ銃をターゲットへ向けるスクリプト
    /// キャラクターの腕の逆運動学を数値計算敵にで解決しながら、スムーズに動作させる
    /// 2Dキャラクターにはboneが設定済みであり、各boneの逆運動学を解く事で動きを実現
    /// (自作した n - linkゲームオブジェクトの逆運動学を解くスクリプトを作用させる)
    /// アニメーション無し。
    /// </summary>

    [SerializeField]
    private Transform _target;          // 照準：ターゲット

    // ---- 逆運動学を解く対象を設定 ----
    [SerializeField]
    private List<GameObject> _arms_r;       // 右腕ゲームオブジェクトリスト
    [SerializeField]
    private List<GameObject> _arms_l;       // 左腕ゲームオブジェクトリスト
   
    // ---- 逆運動学の目標位置を設定 ----
    [SerializeField]
    private List<GameObject> _arms_r_TG;    // 各右腕ゲームオブジェクトの目標位置リスト
    [SerializeField]
    private List<GameObject> _arms_l_TG;    // 各左腕ゲームオブジェクトの目標位置リスト

    // ---- 逆運動学の目標位置へ追従させるリンクを設定 ----
    [SerializeField]
    private List<GameObject> _arms_r_EE;
    [SerializeField]
    private List<GameObject> _arms_l_EE;
   
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        //transform.position += new Vector3(horizontal * Time.deltaTime * 5, 0, 0);
        // 逆運動学の数値解を各モジュールで得る
        for(int i = 0; i < _arms_r.Count; i++)
        {   // 右腕モジュール計算実行
            _arms_r[i].GetComponent<n_MAS>().CalcIKPosAll(Time.deltaTime, _arms_r_EE[i].transform, _arms_r_TG[i].transform);
           
        }
        for(int i = 0; i < _arms_l.Count; i++)
        {
            // 左腕モジュール計算実行
            _arms_l[i].GetComponent<n_MAS>().CalcIKPosAll(Time.deltaTime, _arms_l_EE[i].transform, _arms_l_TG[i].transform);
        }
        // キャラの向きに応じてスケール変更(x)
        if(_target.transform.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        } else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
