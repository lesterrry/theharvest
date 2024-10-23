using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.Platformer
{
    [RequireComponent(typeof(CharacterLogic))]
    [RequireComponent(typeof(Animator))]
    public class CharacterAnim : MonoBehaviour
    {
        private CharacterLogic character;
        private CharacterHoldItem character_item;
        private Animator animator;

        void Awake()
        {
            character = GetComponent<CharacterLogic>();
            character_item = GetComponent<CharacterHoldItem>();
            animator = GetComponent<Animator>();
        }

        void Update()
        {

            //Anims
            // animator.SetBool("Jumping", character.IsJumping());
            // animator.SetBool("InAir", !character.IsGrounded());
            // animator.SetBool("Crouching", character.IsCrouching());
            animator.SetFloat("Speed", character.GetMove().magnitude);
            // if (character_item != null)
            //     animator.SetBool("Hold", character_item.GetHeldItem() != null);

        }

        void OnCrouch()
        {
            animator.SetTrigger("Crouch");
        }

        void OnJump()
        {
            animator.SetTrigger("Jump");
        }
    }

}