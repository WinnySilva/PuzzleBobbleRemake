using UnityEngine;

namespace Animation
{
    public class DinoDireita : MonoBehaviour
    {
        public void AnimacaoRodarAlavanca(bool esquerda,bool liga)
        {
            Animator anim;
            if (TryGetComponent(out anim))
            {
                anim.SetBool("MiraMovendo", liga);
            }
        }
    }
}