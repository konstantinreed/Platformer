using System;

namespace GameLibrary
{
	public class InputState : IStepState
	{
		private const float JumpPressedDurability = 4;

		public int Step { get; set; }

		public bool IsLeftPressed;
		public bool IsRightPressed;
		public bool IsJumpPressed;
		internal int JumpPressedStep;
		public bool IsHitPressed;
		public bool IsSuicidePressed;

		internal bool IsJumpJustPressed => IsJumpPressed && JumpPressedStep >= Step - JumpPressedDurability;

		public void Reset()
		{
			IsLeftPressed = default(bool);
			IsRightPressed = default(bool);
			IsJumpPressed = default(bool);
			JumpPressedStep = default(int);
			IsHitPressed = default(bool);
			IsSuicidePressed = default(bool);
		}

		internal void Copy(InputState state)
		{
			IsLeftPressed = state.IsLeftPressed;
			IsRightPressed = state.IsRightPressed;
			IsJumpPressed = state.IsJumpPressed;
			JumpPressedStep = state.JumpPressedStep;
			IsHitPressed = state.IsHitPressed;
			IsSuicidePressed = state.IsSuicidePressed;
        }

		internal void Merge(InputState state)
		{
            IsLeftPressed = IsLeftPressed || state.IsLeftPressed;
			IsRightPressed = IsRightPressed || state.IsRightPressed;
			IsJumpPressed = IsJumpPressed || state.IsJumpPressed;
			JumpPressedStep = Math.Max(JumpPressedStep, state.JumpPressedStep);
			IsHitPressed = IsHitPressed || state.IsHitPressed;
			IsSuicidePressed = IsSuicidePressed || state.IsSuicidePressed;
		}

		internal void SetPreviousInput(InputState previousInput)
		{
			if (IsJumpPressed && !previousInput.IsJumpPressed) {
				JumpPressedStep = Step;
            } else {
				JumpPressedStep = previousInput.JumpPressedStep;
            }
		}

		internal bool IsDiffersFrom(InputState state)
		{
			return
				(!IsLeftPressed && state.IsLeftPressed) ||
				(!IsRightPressed && state.IsRightPressed) ||
				(!IsJumpPressed && state.IsJumpPressed) ||
				(!IsHitPressed && state.IsHitPressed) ||
				(!IsSuicidePressed && state.IsSuicidePressed);
        }

		public static bool operator==(InputState lhs, InputState rhs)
		{
			if (ReferenceEquals(lhs, rhs)) {
				return true;
			}
			if (((object)lhs == null) || ((object)rhs == null)) {
				return false;
			}

			return
				lhs.IsLeftPressed == rhs.IsLeftPressed &&
				lhs.IsRightPressed == rhs.IsRightPressed &&
				lhs.IsJumpPressed == rhs.IsJumpPressed &&
				lhs.IsHitPressed == rhs.IsHitPressed &&
				lhs.IsSuicidePressed == rhs.IsSuicidePressed;
		}

		public static bool operator!=(InputState lhs, InputState rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object obj)
		{
			return this == (obj as InputState);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
