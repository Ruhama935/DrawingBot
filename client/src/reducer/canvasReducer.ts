import type { DrawCommand } from '../models/DrawCommand'
import type { CommandGroup } from '../models/CommandGroup'

export type State = {
  history: CommandGroup[]
  currentIndex: number
  savedDrawings: CommandGroup[],
  userIn: boolean
}

export type Action =
  | { type: 'APPLY'; commands: DrawCommand[]; prompt: string }
  | { type: 'UNDO' }
  | { type: 'REDO' }
  | { type: 'CLEAR' }
  | { type: 'LOAD_SAVED_DRAWING'; drawings: CommandGroup[] }
  | { type: 'SET_USER_IN'; in: boolean }

export const initialState: State = {
  history: [],
  currentIndex: -1,
  savedDrawings: [],
  userIn: false
}

export function canvasReducer(state: State, action: Action): State {
  switch (action.type) {
    case 'APPLY': {
      const history = state.history.slice(0, state.currentIndex + 1)

      history.push({
        prompt: action.prompt,
        commands: action.commands
      })

      return {
        ...state,
        history,
        currentIndex: history.length - 1
      }
    }

    case 'UNDO':
      return {
        ...state,
        currentIndex: Math.max(state.currentIndex - 1, -1)
      }

    case 'REDO':
      return {
        ...state,
        currentIndex: Math.min(
          state.currentIndex + 1,
          state.history.length - 1
        )
      }

    case 'LOAD_SAVED_DRAWING': {
      const drawings = [...action.drawings];
      return {
        ...state,
        savedDrawings: drawings
      }
    }


    case 'CLEAR':
      return {
        ...state,
        history: [],
        currentIndex: -1
      }

    case 'SET_USER_IN':
      return {
        ...state,
        userIn: action.in
      }
      
    default:
      return state
  }
}
