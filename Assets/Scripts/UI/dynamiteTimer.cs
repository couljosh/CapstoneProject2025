using UnityEngine;

public class dynamiteTimer : MonoBehaviour
{
    public Animator animator;
    public AnimationClip clip;
    private float time;
    public SceneChange SceneChange;
    private float lengthOfAnim;
    private float speedMulitplier;
    private bool startAnimation;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lengthOfAnim = clip.length;
        
        time = SceneChange.roundTime;

        speedMulitplier =  lengthOfAnim / time;

        animator.SetFloat("speedMulitplier", speedMulitplier);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    void StartAnimation()
    {
        animator.SetBool("StartTimer",true);
    }


}
