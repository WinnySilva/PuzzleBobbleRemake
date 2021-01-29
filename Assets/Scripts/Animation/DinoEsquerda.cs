using UnityEngine;

namespace Animation
{
    public class DinoEsquerda : MonoBehaviour
    {
        public void AnimacaoHurry(bool liga)
        {
            Animator anim;
            if (TryGetComponent(out anim))
            {
                anim.SetBool("ApressarJogador", liga);
            }
        }
    }
}