using UnityEngine;
using Elympics;

public class JumpManager : ElympicsMonoBehaviour
{
    private const float GroundCheckOffset = 0.01f;

    [Header("References")]
    [SerializeField] private Rigidbody2D playerRigidBody;
    public Rigidbody2D PlayerRigidbody => playerRigidBody;
    [SerializeField] private BoxCollider2D playerCollider;
    [Header("Jump Variables")]
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float defaultGravity;
    [SerializeField] private float lowJumpMultiplier;
    [SerializeField] private float highJumpMultiplier;
    [SerializeField] private float fallingMultiplier;
    [SerializeField] private LayerMask groundMask;

    [Header("Quality of life")]
    [SerializeField] private float coyoteTime;
    private float coyoteTimeCounter;
    [SerializeField] private float jumpBufferDuration;
    private float jumpBufferCounter;
    private bool rememberedJumpInput;

    public void ManageJump(bool jumpInput)
    {
        //We only jump when the jump button was pressed, not held
        bool startJump = false;
        if (!rememberedJumpInput && jumpInput) startJump = true;
        rememberedJumpInput = jumpInput;

        //Jump buffering
        if (startJump) jumpBufferCounter = jumpBufferDuration;

        //Checking if we're grounded + coyote time
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }

        float gravityMultiplier;
        if (coyoteTimeCounter > 0 && jumpBufferCounter > 0)
        {
            //Actual jump
            playerRigidBody.velocity = Vector2.up * jumpVelocity;
            gravityMultiplier = 0;
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
        }
        else if (playerRigidBody.velocity.y < 0)
        {
            //Faster falling
            gravityMultiplier = fallingMultiplier;
        }
        else if (playerRigidBody.velocity.y > 0 && !jumpInput)
        {
            //Low jump
            gravityMultiplier = lowJumpMultiplier;
        }
        else
        {
            //High jump
            gravityMultiplier = highJumpMultiplier;
        }

        playerRigidBody.velocity += gravityMultiplier * defaultGravity * Elympics.TickDuration * Vector2.up;

        coyoteTimeCounter -= Elympics.TickDuration;
        jumpBufferCounter -= Elympics.TickDuration;
    }

    public bool IsGrounded()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(playerRigidBody.position + GroundCheckOffset * Vector2.down, PlayerSizeScaled, 0, groundMask);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<PlatformEffector2D>(out var _))
            {
                if (hit.bounds.max.y <= playerCollider.bounds.min.y) return true;
            }
            else return true;
        }
        return false;
    }

    private Vector2 PlayerSizeScaled => Vector2.Scale(transform.localScale, playerCollider.size);
}
