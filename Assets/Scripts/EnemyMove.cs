using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rb2D;              // ヘリのリジッドボディ2D取得
    [SerializeField]
    private Transform _heliTargetPos;       // ヘリの位置PID制御目標位置
    // 位置制御用パラメータ
    Vector3 _preErrorPos = new Vector3(0, 0, 0);
    Vector3 _integralError = new Vector3(0, 0, 0);
    [SerializeField]
    float _velMax = 5f;     // 速度限界
    [SerializeField]
    float _kp = 16f;        // P制御係数  
    [SerializeField]
    float _ki = 0.01f;      // I制御係数
    [SerializeField]
    float _kd = 3.5f;       // D制御係数
    [SerializeField]
    float _angleLim = 20f;  // ヘリ傾斜角度限界(deg) 
    [SerializeField]
    float _dth = 1f;  // ヘリ傾斜角度制御係数
 
    private void FixedUpdate()
    {
        // ヘリを位置＆姿勢制御（ふわふわ飛んでいる感じを再現＆制御係数で良い感じの調整にしたい)
        HeliController(Time.deltaTime, _heliTargetPos);
    }

    private void HeliController(float delta_time, Transform targetPos)
    {
        // --- ヘリコプターの位置PID制御 --- //
        // -- 誤差計算 -- //
        Vector3 posError = targetPos.position - transform.position;
        // -- 制御入力生成 -- //
        _integralError = (posError + _preErrorPos) / 2 * delta_time;
        Vector3 uf =    _kp * posError + 
                        _ki * _integralError + 
                        _kd * (posError - _preErrorPos) / delta_time;
        // -- 制御入力をリジッドボディへ反映 -- //
        Vector2 uf2D = uf;  // 3D -> 2D(x, y)落とし込み
        _rb2D.AddForce(uf);
        // 速度制限
        if(_rb2D.velocity.magnitude >= _velMax)
        {
            _rb2D.velocity = _rb2D.velocity.normalized * _velMax;
        }

        _preErrorPos = posError;    // 今フレームの誤差を格納しておく

        // --- ヘリコプターの姿勢水平制御 --- //
        // -- 水平移動中の速度に応じて目標傾斜角度変化 

        Vector3 rotEuler = this.transform.rotation.eulerAngles;
        float rotz = 0;
        // 角度を180 ~ -180(deg)に変換
        if(rotEuler.z > 180)
        {
            rotz = rotEuler.z - 360f;
        } else
        {
            rotz = rotEuler.z;
        }

        // 前進中
        if (_rb2D.velocity.x >= 0)
        {
            // 前傾角度にしたい
            if (rotz >= -_angleLim)
            {
                transform.Rotate(0, 0, -_dth);
            }
            else if (rotz < -_angleLim)
            {
                transform.Rotate(0, 0, _dth);
            }
        }
        else if (_rb2D.velocity.x < 0)
        {
            // 後傾角度にしたい
            if (rotz >= _angleLim)
            {
                transform.Rotate(0, 0, -_dth);
            }
            else if (rotz < _angleLim)
            {
                transform.Rotate(0, 0, _dth);
            }
        }

    }

}
