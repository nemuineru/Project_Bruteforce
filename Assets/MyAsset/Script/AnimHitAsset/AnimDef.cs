

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using System.Linq;
//using UnityEditor.Experimental.GraphView;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using Unity.VisualScripting;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityDebug;
using Animancer;

//Animancer版. Animancerで出来る事がいっぱいあるみたいなので一任する
public class AnimancerManager
{
    //AnimancerManagerが取り扱うEntity.
    public Entity root;
    public AnimancerComponent main;
    //Layer[0]で読み出されるメインノード.
    //clssが読み出す当たり判定要素は基本、これを使う.
    public AnimDef primaryAnimDef;

    //mainAnimancerのステート遷移システム
    public void TransitLayer(AnimDef animDef, int LayerID = 0, float TimeOffset = 0f, AvatarMask animMask = null, bool isAdditive = false)
    {
        bool isMaskExist = animMask != null;
        //Layerを指定。余計なことせずに指定して例外が出ないのはやっぱ便利よ
        AnimancerLayer inserterLayer = main.Layers[LayerID];

        //Transitの指定時、余分なStateを消し飛ばす.
        for (int i = 0; i < inserterLayer.ChildCount; i++)
        {
            AnimancerState selState = inserterLayer.GetChild(i);
            if (selState.IsActive == false)
            {
                selState.Destroy();
            }
        }

        //Maskが存在すればそれを適用する.
        if (isMaskExist)
        {
            inserterLayer.Mask = animMask;
        }
        //基本値はOverRide.
        inserterLayer.IsAdditive = isAdditive;

        inserterLayer.Play(MakeState(animDef), animDef.blendInTime);

        //if the layerID is 0, clss must loaded  via layer 0 so call the order.
        if (LayerID == 0)
        {
            primaryAnimDef = animDef;
        }
    }

    //AnimancerStateをanimDefより作成
    public AnimancerState MakeState(AnimDef animDef)
    {
        //Create the state and returns it.
        AnimancerState MakeState;
        //switch for animdef cases. ..which contains the clssDefs
        switch (animDef.mixType)
        {
            case AnimDef.MixType.Liner:
                {
                    MakeState = new LinearMixerState();
                    foreach (AnimDef.Anims anims in animDef.animClip)
                    {
                        ((LinearMixerState)MakeState).Add
                        (anims.Clip, anims.mixPosition.x);
                    }
                    break;
                }
            case AnimDef.MixType.FreeForm2D:
                {
                    MakeState = new DirectionalMixerState();
                    foreach (AnimDef.Anims anims in animDef.animClip)
                    {
                        ((DirectionalMixerState)MakeState).Add
                        (anims.Clip, anims.mixPosition);
                    }
                    break;
                }
            case AnimDef.MixType.Cartesian2D:
                {
                    MakeState = new CartesianMixerState();
                    foreach (AnimDef.Anims anims in animDef.animClip)
                    {
                        ((CartesianMixerState)MakeState).Add
                        (anims.Clip, anims.mixPosition);
                    }
                    break;
                }
            case AnimDef.MixType.Direct:
            default:
                {
                    MakeState = new LinearMixerState();
                    foreach (AnimDef.Anims anims in animDef.animClip)
                    {
                        ((LinearMixerState)MakeState).Add
                        (anims.Clip, anims.mixPosition.x);
                    }
                    break;
                }
        }
        return MakeState;
    }

    //float又はVector2で処理する値を変える.
    //Layerのパラメータ値を変更..
    public void AM_AnimParamSet(Vector2 paramVect, int LayerID = 0, int subStateID = -1, float stateParam = -1)
    {
        AnimancerLayer processLayer = main.Layers[LayerID];
        if (processLayer != null)
        {
            AnimancerState currentMjState = processLayer.CurrentState;
            if (subStateID > -1)
            {

            }
            switch (currentMjState)
            {
                //for linear Mxer, use paramVect.x
                case LinearMixerState LMix:
                    {
                        LMix.Parameter = paramVect.x;
                        break;
                    }
                //Directional and Cartesian is using Vector2
                case DirectionalMixerState DMix:
                    {

                        DMix.Parameter = paramVect;
                        break;
                    }
                case CartesianMixerState CMix:
                    {
                        CMix.Parameter = paramVect;
                        break;
                    }
                //otherwise, DirectParameter Changer is selected
                default:
                    {
                        break;
                    }
            }
        }
    }

    //時空を操る. ..ということではなくステート遷移を一本で纏めたやつ.
    //playSpeedを0にすれば、一時的なpauseとなる.
    public void AM_AnimPlayStateSet(float playTime, float playSpeed = 1.0f, int LayerID = 0, int subStateID = -1)
    {
        AnimancerLayer processLayer = main.Layers[LayerID];
        if (processLayer != null)
        {
            AnimancerState currentMjState = processLayer.CurrentState;
            //subState will selected if the ID is provided.
            if (subStateID > -1)
            {

            }
            else
            {
                //get every AnimancerState that mixed as currentMjState. I guess single state will return itself so not bugged?
                //..if the AnimancerState is Mixer. GetChildState wont work if it is singleton.
                for (int i = 0; i < currentMjState.ChildCount; i++)
                {
                    AnimancerState st = currentMjState.GetChild(i);
                    st.Speed = playSpeed;
                    st.Time = playTime;
                }
            }
        }
    }

    //現在の指定レイヤーでの経過時間.
    public float AM_AnimCurrentTime(int LayerID)
    {
        float val = 0;
        AnimancerLayer refLayer = main.Layers[LayerID];
        AnimancerState findState = refLayer.CurrentState;
        if (refLayer != null)
        {
            List<AnimationClip> cls = new List<AnimationClip>();
            refLayer.CurrentState.GatherAnimationClips(cls);
            float frametime = cls.Average(t => t.frameRate);
            val = refLayer.CurrentState.Time * frametime;
        }
        return val;
    }

    //現在の指定レイヤーの推定終了時刻. 再生速度を考慮する.
    public float AM_AnimEndTime(int LayerID)
    {
        float val = 0;
        AnimancerLayer refLayer = main.Layers[LayerID];
        AnimancerState findState = refLayer.CurrentState;
        if (refLayer != null)
        {
            List<AnimationClip> cls = new List<AnimationClip>();
            refLayer.CurrentState.GatherAnimationClips(cls);
            float frametime = cls.Average(t => t.frameRate);
            val = refLayer.CurrentState.Length * frametime;;
            //Debug.Log(val);
        }
        return val;
    }

    //レイヤーのフェードアウト.
    //作成したレイヤーは基本的に消去不可能なので、ベースレイヤー以外はウェイト0に設定する.
    public void AM_FadeLayer(int LayerID = 0, float fadeTime = 0)
    {
        AnimancerLayer refLayer = main.Layers[LayerID];
        if (refLayer != null)
        {
            refLayer.StartFade(fadeTime);
            //Debug.Log(val);
        }
    }

    //Set Animancer TickStates.
    public float Tick(bool isPause)
    {
        float resumeSpeed = 1f;
        foreach (AnimancerLayer lA in main.Layers)
        {
            if (isPause)
            {
                lA.Speed = resumeSpeed;
            }
            else
            {
                lA.Speed = 0;
            }
        }

        if (primaryAnimDef != null)
        {
            //update Capsule Positions.
            primaryAnimDef.initEntityAt(root);
            primaryAnimDef.checkClssCapsule();
        }
        return 0;
    }
}

//animDefには以下を登録 - 
// アニメーションID, アニメーション名, アニメーションの速度, 
// アニメーションの基本ブレンド（イン・アウト）タイムなど
// また、基礎の当たり判定リスト(カプセル型)を登録.
// 2025-06-12
// clssの表示設定もやらねば.
[System.Serializable]
public class AnimDef
{
    [System.Serializable]

    //ミキシングするアニメのウェイトなど.
    public class Anims
    {
        public Anims(AnimationClip clip) => Clip = clip;
        public AnimationClip Clip;
        public float speed = 1f, startFrame, cycleOffset;
        public Vector2 mixPosition = Vector2.zero;

        public AnimationClipPlayable clipPlayable;

        public float MixWeightSet
        {
            get
            {
                return _MixWeight;
            }
            set
            {
                _MixWeight = value;
            }
        }
        float _MixWeight = 1f;
    }

    struct clipPosStatement
    {
        internal clipPosStatement(int id, Vector2 p)
        {
            index = id;
            pos = p;
            WeightSet = 1f;
            Angle = 0f;
        }
        internal int index;
        internal float WeightSet;
        internal Vector2 pos;

        internal float Angle;
    }

    //ReWrite for Animancer. This gonna be more easier than hit the AnimationAPI directly.
    public void ChangeWeight(float BaseWeight)
    {
        //
        if(animClip.Length <= 1)
        {
            animClip[0].MixWeightSet = BaseWeight;
            return;
        }


    }

    public enum MixType
    {
        Direct,
        Liner,
        FreeForm2D,
        Cartesian2D
    }

    public MixType mixType = MixType.Liner;
    public string MixParamName;
    public Vector2 CurrentParamPos;

    //public AnimatorControllerLayer playLayer;
    public int ID;
    public string Name;

    public Anims[] animClip;
    public float blendInTime, blendOutTime;

    float DefWeight = 1f;

    public bool useDefaultClss;    

    //アニメーションごとにオーバーライド・設定可能な判定をここで設定する.
    //null値が挿入されているならデフォルトを使用..と考える.
    [SerializeField]
    public clssSetting clssSetting = new clssSetting();


    //clsssettingにEntityを登録.
    public void initEntityAt(Entity init)
    {
        clssSetting.initClss(init);
    }

    //カプセルの描写
    public void checkClssCapsule()
    {
        clssSetting.clssPosUpdate();
        List<clssDef> ca = clssSetting.findclss(clssDef.ClssType.Hit, 0f);
        ca.AddRange(clssSetting.findclss(clssDef.ClssType.Attack, 0f));
        
        foreach (clssDef c in ca)
        {
            //Debug.Log("clssCapsule name : " + c.attachTo);
            //Debug.Log(c.attachTo);
            c.getGlobalPos();
            c.DrawCapsule();
            if (c.showGizmo == true)
            {
                if (c.clssType == clssDef.ClssType.Attack)
                {
                    //Debug.Log("attackCapsule");
                }
                //c.DrawCapsule();
            }
        }
    }

    //Cloneメソッドの拡張..
    public AnimDef Clone() => new AnimDef
    {
        //AnimClipはそのまま..
        animClip = this.animClip,
        mixType = this.mixType,
        MixParamName = this.MixParamName,
        ID = this.ID,
        Name = this.Name,
        blendInTime = this.blendInTime,
        blendOutTime = this.blendOutTime,
        useDefaultClss = this.useDefaultClss,
        clssSetting = this.clssSetting.Clone()
    };
}


[System.Serializable]
public class clssSetting
{
    //アニメーションごとにオーバーライド・設定可能な判定をここで設定する.
    //ここで記述されたclssは調整済みのデフォルトに挿入する形で設定される
    //null値が挿入されているならなにもしない.
    public List<clssDef> clssDefs = new List<clssDef>();

    //デフォルトClssを除去するためのリスト. 
    //除去指定をデフォルトのClssのオブジェ名称から消す.
    public List<string> disableClssList = new List<string>();
    public Entity root;

    //投げとかステート奪取時の挙動用. 一時的に当たり判定を除去する.
    public bool eraseAllClss;

    //entityで読み出すclssにentityを用意する.
    public void initClss(Entity entity)
    {
        root = entity;
        foreach (clssDef c in clssDefs)
        {
            c._entity = root;
            c.initTransform(c._entity);
        }
    }

    public void clssPosUpdate()
    {
        foreach (clssDef cls in clssDefs)
        {
            cls.updatelastPos();
        }
    }

    //使用する当たり判定を用意する.
    public List<clssDef> findclss(clssDef.ClssType useType, float frame)
    {
        //clssDef内のclssを用意.
        List<clssDef> findDefs = new List<clssDef>();
        foreach (clssDef cls in clssDefs)
        {
            if (cls.StartTime <= frame || cls.EndTime > frame && cls.clssType == useType)
            {
                findDefs.Add(cls);
            }
        }
        if (root != null)
        {
            //Entity内のclssを用意. 除外対象をdisableClssから検索して削除.
            //AnimClipのEraseAllClssが設定されているなら消し飛ばす.
            foreach (clssDef defaultcls in root.defaultClss.clssDefs)
                if (defaultcls.clssType == useType &&
                !disableClssList.Any(dz => dz == defaultcls.attachTo)
                && eraseAllClss == false)
                {
                    findDefs.Add(defaultcls);
                }
        }
        //初期ロード時rootが登録されていない. 何故だろう？
        else
        {
            Debug.Log("cant find default");
        }
        return findDefs;
    }

    //clss同士の接触判定を行う.
    public bool clssCollided(out Vector3 v1, out Vector3 v2, out float dist, clssDef.ClssType useType,
    clssSetting compareTo, float frame)
    {
        //clssRef <= 呼び出し側の比較するclssカプセル.
        List<clssDef> clssRef = new List<clssDef>();
        //clssCompareTO <= 比較対象の使用するclssカプセル.
        List<clssDef> clssCompareTo = new List<clssDef>();
        v1 = Vector3.zero;
        v2 = Vector3.zero;
        dist = Mathf.Infinity;
        bool isCollided_any = false;
        if (useType == clssDef.ClssType.Attack)
        {
            clssRef = findclss(useType, frame);
            clssCompareTo = compareTo.findclss(clssDef.ClssType.Hit, frame);
        }
        else
        {
            clssRef = findclss(useType, frame);
            clssCompareTo = compareTo.findclss(clssDef.ClssType.Attack, frame);
        }

        foreach (clssDef cls in clssRef)
        {
            foreach (clssDef compcls in clssCompareTo)
            {
                //cls.DrawUPos(Color.red);
                float compareing;
                Vector3 v_a, v_b;
                bool isCollided = cls.isCollided(out v_a, out v_b, out compareing, compcls);
                if (isCollided && compareing < dist)
                {
                    v1 = v_a;
                    v2 = v_b;
                    compareing = dist;
                    isCollided_any = true;
                }
            }
        }
        return isCollided_any;
    }

    public bool clssCollided(out Vector3 v1, out Vector3 v2, out float dist, clssDef.ClssType useType,
    clssSetting compareTo, float frame, Entity entity_self, Entity entity_compareTo)
    {
        List<clssDef> clssRef = new List<clssDef>();
        List<clssDef> clssCompareTo = new List<clssDef>();
        v1 = Vector3.zero;
        v2 = Vector3.zero;
        dist = Mathf.Infinity;
        bool isCollided_any = false;
        if (useType == clssDef.ClssType.Attack)
        {
            clssRef = findclss(useType, frame);
            clssCompareTo = compareTo.findclss(clssDef.ClssType.Hit, frame);
        }
        else
        {
            clssRef = findclss(useType, frame);
            clssCompareTo = compareTo.findclss(clssDef.ClssType.Attack, frame);
        }
        foreach (clssDef cls in clssRef)
        {
            foreach (clssDef compcls in clssCompareTo)
            {
                float compareing;
                Vector3 v_a, v_b;
                bool isCollided = cls.isCollided(out v_a, out v_b, out compareing, compcls);
                if (isCollided && compareing < dist)
                {
                    v1 = v_a;
                    v2 = v_b;
                    compareing = dist;
                    isCollided_any = true;
                }
            }
        }
        return isCollided_any;
    }


    //Cloneメソッドの拡張..
    public clssSetting Clone()
    {
        //新規に作成.
        var retDef = new clssSetting();
        foreach (clssDef c in this.clssDefs)
        {
            retDef.clssDefs.Add(c.Clone);
        }
        //string形式なので自動的にdeepcopyされるはず、というか参照でもいいのか？
        retDef.disableClssList.AddRange(this.disableClssList);
        retDef.eraseAllClss = this.eraseAllClss;

        return retDef;
    }
}

[System.Serializable]
[SerializeField]
public class clssDef
{
    //Cloneの拡張
    public clssDef Clone => new clssDef
    {
        clssType = this.clssType,
        attachTo = this.attachTo,
        showGizmo = this.showGizmo,
        startPos = this.startPos,
        endPos = this.endPos,
        width = this.width,
        StartTime = this.StartTime,
        EndTime = this.EndTime        
    };

    //やられ判定と当たり判定をenumで管理する
    public enum ClssType
    {
        Hit,
        Attack
    }
    public ClssType clssType;
    //対象キャラクターのどのゲームオブジェクトに追随するか..を取得し、下のattachTransformに値を入力.
    public string attachTo;
    public bool showGizmo;
    public Entity _entity;

    public Transform attachTransform;

    //wを半径とする.
    //スタートポジション・終了ポジションはtransformを基準とする.
    //attachTransformが存在しない際は
    [SerializeField]
    internal Vector3 startPos, endPos;
    [SerializeField]
    internal float width;

    //前回のスタート・終了ポジションを設定.
    public Vector3 _lastcalcStartPos = Vector3.zero;
    public Vector3 _lastcalcEndPos = Vector3.zero;

    //当たり判定設定時間 - 設定終了時間を表す.
    internal float StartTime = 0f, EndTime = 10f;

    //基準Transformを軸にしたカプセルコライダの設定
    public (Vector3, Vector3) getGlobalPos()
    {
        Vector3 start, end;
        if (attachTransform != null)
        {
            Transform t = attachTransform;

            start = t.position + t.rotation * startPos;
            end = t.position + t.rotation * endPos;
        }
        else
        {
            start = startPos;
            end = endPos;
        }
        return (start, end);
    }

    public void DrawUPos(UnityEngine.Color color)
    {
        Vector3 v0_1, v0_2;
        Vector3 v1_1 = _lastcalcStartPos, v1_2 = _lastcalcEndPos;
        (v0_1, v0_2) = getGlobalPos();
        
        Debug.DrawLine(v0_1,v0_2,color);
        Debug.DrawLine(v1_1,v1_2,color);
        Debug.DrawLine(v0_1,v1_1,color);
        Debug.DrawLine(v0_2,v1_2,color);
    }

    //当たり判定指定
    //Entityが無いやつに関して..
    public void setTransform(Transform tfm)
    {
        attachTransform = tfm;
    }

    //entity自体のトランスフォームを読み出し.
    //attachToが存在しない際はentityのルートを選択する.
    public void initTransform(Entity entity)
    {
        _entity = entity;
        attachTransform = entity.transform;
        if (attachTo != null)
        {
            //entity中のすべての階層のtransformを取得.
            //一度しかやらないことを想定..
            Transform[] transforms = entity.allChildTransforms;
            foreach (Transform t in transforms)
            {
                if (t.name == attachTo)
                {
                    attachTransform = t;
                    return;
                }
            }
        }
    }

    public void DrawCapsule()
    {
        Vector3 pos_1, pos_2;
        (pos_1, pos_2) = getGlobalPos();
        //Debug.Log(attachTo + " Drawin Capsules at" + pos_1.ToString() + pos_2.ToString());
        DrawCapsuleGizmo_Tool(pos_1, pos_2, width,
        clssType == ClssType.Hit ? Color.blue : Color.red);
        Debug.DrawLine(pos_1,pos_2);
    }

//Gizmoの描写
    static public void DrawCapsuleGizmo_Tool(Vector3 start, Vector3 end, float radius, Color col)
    {
        int size = (int)((end - start).magnitude / radius);
        DrawWireSphere_OnDebug(start, radius, col);
        DrawWireSphere_OnDebug(end, radius, col);
        if (radius > 0)
        {
            //Debug.Log("Width : " + radius);
        }
        for (int i = 1; i < size + 2; i++)
            {
                //Debug.Log("Drawin Capsules");
                Vector3 DrawAt = start + (end - start) * ((float)i / Mathf.Max(1, size + 2));
                //Debug.Log("Drawin Sphere at" + DrawAt);
                DrawWireSphere_OnDebug(DrawAt, radius, col);
            }
    }

    static public void DrawWireSphere_OnDebug(Vector3 pos, float radius, Color col)
    {
        int segs = 12;
        //Debug.Log(pos);
        for (int i = 0; i < segs; i++)
        {
            Vector3 drawer_1 = pos +
            Quaternion.AngleAxis(360f * i / segs, Vector3.up) * (Vector3.right * radius);
            Vector3 drawer_2 = pos +
            Quaternion.AngleAxis(360f * (i + 1) / segs, Vector3.up) * (Vector3.right * radius);
            Debug.DrawLine(drawer_1, drawer_2, col);
        }
        for (int i = 0; i < segs; i++)
        {
            Vector3 drawer_1 = pos +
            Quaternion.AngleAxis(360f * i / segs,Vector3.right) * (Vector3.up * radius);
            Vector3 drawer_2 = pos +
            Quaternion.AngleAxis(360f * (i + 1) / segs,Vector3.right) * (Vector3.up * radius);
            Debug.DrawLine(drawer_1,drawer_2, col);
        }
        for (int i = 0; i < segs; i++)
        {
            Vector3 drawer_1 = pos +
            Quaternion.AngleAxis(360f * i / segs,Vector3.forward) * (Vector3.right * radius);
            Vector3 drawer_2 = pos +
            Quaternion.AngleAxis(360f * (i + 1) / segs,Vector3.forward) * (Vector3.right * radius);
            Debug.DrawLine(drawer_1,drawer_2, col);
        }
    }

    //最後のポジションの計算
    public void updatelastPos()
    {
        (_lastcalcStartPos, _lastcalcEndPos) = getGlobalPos();
    }

    //当たり判定の計算
    public bool isCollided(out Vector3 v1 , out Vector3 v2 , out float dist , clssDef compareTo)
    {
        v1 = Vector3.zero;
        v2 = Vector3.zero;
        dist = Mathf.Infinity;
        (Vector3 midPos_1, float base_distToMid) = GetMid();
        (Vector3 midPos_2, float compare_distToMid) = compareTo.GetMid();
        if ((midPos_2 - midPos_1).magnitude <= (base_distToMid + width + compare_distToMid + compareTo.width))
        {
            //ボール型の当たり判定概形（要は平行四面 + カプセルスイープの範囲を考慮した最小最接球を導出.）
            //Gizmos.DrawSphere(midPos_1, base_distToMid + width);
            //Gizmos.DrawSphere(midPos_1, compare_distToMid + compareTo.width);

            TrisUtil.Triangle T1_0, T1_1 , T2_0 , T2_1;
            (T1_0, T1_1) = GetTriangle_MovedPlane();
            (T2_0, T2_1) = compareTo.GetTriangle_MovedPlane();

            T1_0.DrawTris();
            T1_1.DrawTris();
            T2_0.DrawTris();
            T2_1.DrawTris();


            /*
            //デバッグ用に表示.
            //
            //2025-06-11 : 三角形の配列形式表示で見たところ、全配列でvectorが0になってたのでsetvs()をコールするようにした.
            string g = "" , f = "";
            int i = 0;
            foreach (Vector3 posList_T1 in T1_0.vs)
            {
                f += string.Format("pos{0} : {1}", i, posList_T1);
                i++;
            }
            foreach (Vector3 posList_T2 in T2_0.vs)
            {
                g += string.Format(" pos{0} : {1}", i, posList_T2);
                i++;
            }
            Debug.Log(f + g);
            */

            //v1は自己の当たり判定位置　v2は相手の.
            getLeastPos(ref dist, ref v1, ref v2, T1_0, T2_0);
            getLeastPos(ref dist, ref v1, ref v2, T1_1, T2_0);
            getLeastPos(ref dist, ref v1, ref v2, T1_0, T2_1);
            getLeastPos(ref dist, ref v1, ref v2, T1_1, T2_1);

            //Debug.Log(dist);


            if (dist <= width + compareTo.width)
            {
                return true;
            }
        }
        return false;
    }
    
    //三角面同士の最短距離導出
    public void getLeastPos(ref float last, ref Vector3 v1, ref Vector3 v2, TrisUtil.Triangle t1, TrisUtil.Triangle t2)
    {
        float d1;
        Vector3 v1_ch, v2_ch;
        TrisUtil TU = new TrisUtil();
        (d1, v1_ch, v2_ch) = TU.TrisDistance(t1, t2);
        if (last > d1)
        {
            last = d1;
            v1 = v1_ch;
            v2 = v2_ch;
        }
    }

    //動作方向の平行四面近似する三角面の導出
    public (TrisUtil.Triangle, TrisUtil.Triangle) GetTriangle_MovedPlane()
    {
        Vector3 stPos, ePos;
        (stPos, ePos) = getGlobalPos();
        //TrisUtil.Triangle Compare_1_0 = new TrisUtil.Triangle(stPos, ePos, ePos);
        //TrisUtil.Triangle Compare_1_1 = new TrisUtil.Triangle(stPos, ePos, ePos);
        
        TrisUtil.Triangle Compare_1_0 = new TrisUtil.Triangle(stPos, ePos, _lastcalcStartPos);
        TrisUtil.Triangle Compare_1_1 = new TrisUtil.Triangle(ePos, _lastcalcStartPos, _lastcalcEndPos);
        
        //Debug.Log(_lastcalcEndPos.ToString() + _lastcalcStartPos.ToString());
        return (Compare_1_0, Compare_1_1);
    }

    //動作方向の平行四面の中点位置・及び頂点の最長距離を導出.
    public (Vector3, float) GetMid()
    {
        Vector3 st, end;
        (st, end) = getGlobalPos();
        float f_1 = (st - _lastcalcEndPos).sqrMagnitude;
        float f_2 = (end - _lastcalcStartPos).sqrMagnitude;
        float Dist = f_1 > f_2 ? f_1 : f_2;
        return ((st + _lastcalcEndPos) / 2f, Dist);
    }
    
}

