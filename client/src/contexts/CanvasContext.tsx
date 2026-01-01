import { createContext, useContext, useReducer, type ReactNode } from 'react'
import type { DrawCommand } from '../models/DrawCommand'
import { canvasReducer, initialState } from '../reducer/canvasReducer'
import type { CommandGroup } from '../models/CommandGroup'

type CanvasContextType = {
  commands: DrawCommand[];
  prompts: string;
  savedDrawings: CommandGroup[];
  applyNewCommands: (commands: DrawCommand[], prompt: string) => void;
  applyNewSavedDrawings: (drawings: CommandGroup[]) => void;
  undo: () => void;
  redo: () => void;
  clear: () => void;
  canUndo: boolean;
  canRedo: boolean;
  setUserIn: (input: boolean) => void;
  userIn: boolean;
}

const CanvasContext = createContext<CanvasContextType | null>(null)

export function CanvasProvider({ children }: { children: ReactNode }) {
  const [state, dispatch] = useReducer(canvasReducer, initialState)


  const commands =
    state.currentIndex === -1
      ? []
      : state.history
        .slice(0, state.currentIndex + 1)
        .flatMap(group => group.commands)

  const prompts =
    state.currentIndex >= 0
      ? state.history
        .slice(0, state.currentIndex + 1)
        .map(group => group.prompt)
        .join('\n')
      : ''


  const value: CanvasContextType = {
    commands,
    prompts,
    savedDrawings: state.savedDrawings,
    applyNewCommands: (commands, prompt) =>
      dispatch({ type: 'APPLY', commands, prompt }),
    applyNewSavedDrawings: (drawings) =>
      dispatch({ type: 'LOAD_SAVED_DRAWING', drawings }),
    undo: () => dispatch({ type: 'UNDO' }),
    redo: () => dispatch({ type: 'REDO' }),
    clear: () => dispatch({ type: 'CLEAR' }),
    canUndo: state.currentIndex >= 0,
    canRedo: state.currentIndex < state.history.length - 1,
    setUserIn: (inUser: boolean) => dispatch({ type: 'SET_USER_IN', in: inUser }),
    userIn: state.userIn
  }

  return (
    <CanvasContext.Provider value={value}>
      {children}
    </CanvasContext.Provider>
  )
}

export function useCanvas() {
  const ctx = useContext(CanvasContext)
  if (!ctx) throw new Error('useCanvas must be used within CanvasProvider')
  return ctx
}
