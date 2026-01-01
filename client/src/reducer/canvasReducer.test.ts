import { canvasReducer, initialState, type State } from './canvasReducer'
import type { DrawCommand } from '../models/DrawCommand'
import type { CommandGroup } from '../models/CommandGroup'
import { describe, it, expect } from 'vitest'

const cmdA: DrawCommand[] = [
  { type: 'circle', x: 10, y: 10, radius: 5 }
]

const cmdB: DrawCommand[] = [
  { type: 'rect', x: 0, y: 0, width: 10, height: 10 }
]

const mockSavedDrawing: CommandGroup = {
  commands: cmdA,
  prompt: 'A saved drawing'
}

describe('canvasReducer', () => {
  it('APPLY adds command group and advances index', () => {
    const state = canvasReducer(initialState, {
      type: 'APPLY',
      commands: cmdA,
      prompt: 'first'
    })
    expect(state.history.length).toBe(1)
    expect(state.currentIndex).toBe(0)
    expect(state.savedDrawings).toEqual([])
    expect(state.userIn).toBe(false)
    const group = state.history[0]
    expect(group.commands).toEqual(cmdA)
    expect(group.prompt).toBe('first')
  })

  it('APPLY after APPLY appends to history', () => {
    const s1 = canvasReducer(initialState, {
      type: 'APPLY',
      commands: cmdA,
      prompt: 'first'
    })
    const s2 = canvasReducer(s1, {
      type: 'APPLY',
      commands: cmdB,
      prompt: 'second'
    })
    expect(s2.history.length).toBe(2)
    expect(s2.currentIndex).toBe(1)
    expect(s2.history[1].commands).toEqual(cmdB)
  })

  it('UNDO decreases index but not below -1', () => {
    const state: State = {
      history: [
        { prompt: 'a', commands: cmdA }
      ],
      currentIndex: 0,
      savedDrawings: [],
      userIn: false
    }
    const next = canvasReducer(state, { type: 'UNDO' })
    expect(next.currentIndex).toBe(-1)
  })

  it('REDO increases index but not above last item', () => {
    const state: State = {
      history: [
        { prompt: 'a', commands: cmdA },
        { prompt: 'b', commands: cmdB }
      ],
      currentIndex: 1,
      savedDrawings: [],
      userIn: false
    }
    const next = canvasReducer(state, { type: 'REDO' })
    expect(next.currentIndex).toBe(1)
  })

  it('UNDO then APPLY clears future history', () => {
    const state: State = {
      history: [
        { prompt: 'a', commands: cmdA },
        { prompt: 'b', commands: cmdB }
      ],
      currentIndex: 1,
      savedDrawings: [],
      userIn: false
    }
    const undone = canvasReducer(state, { type: 'UNDO' })
    expect(undone.currentIndex).toBe(0)
    const applied = canvasReducer(undone, {
      type: 'APPLY',
      commands: cmdA,
      prompt: 'new'
    })
    expect(applied.history.length).toBe(2)
    expect(applied.currentIndex).toBe(1)
    expect(applied.history[1].prompt).toBe('new')
  })

  it('CLEAR resets to empty state', () => {
    const state: State = {
      history: [
        { prompt: 'x', commands: cmdA }
      ],
      currentIndex: 0,
      savedDrawings: [],
      userIn: false
    }
    const cleared = canvasReducer(state, { type: 'CLEAR' })
    expect(cleared.history).toEqual([])
    expect(cleared.currentIndex).toBe(-1)
    expect(cleared.savedDrawings).toEqual([])
    expect(cleared.userIn).toBe(false)
  })

  it('LOAD_SAVED_DRAWING loads drawings into state', () => {
    const drawings: CommandGroup[] = [mockSavedDrawing]
    const state = canvasReducer(initialState, {
      type: 'LOAD_SAVED_DRAWING',
      drawings
    })
    expect(state.savedDrawings).toEqual(drawings)
    expect(state.savedDrawings.length).toBe(1)
    expect(state.history).toEqual([])
    expect(state.currentIndex).toBe(-1)
  })

  it('LOAD_SAVED_DRAWING preserves existing history', () => {
    const stateWithHistory: State = {
      history: [{ prompt: 'a', commands: cmdA }],
      currentIndex: 0,
      savedDrawings: [],
      userIn: false
    }
    const drawings: CommandGroup[] = [mockSavedDrawing]
    const state = canvasReducer(stateWithHistory, {
      type: 'LOAD_SAVED_DRAWING',
      drawings
    })
    expect(state.savedDrawings).toEqual(drawings)
    expect(state.history.length).toBe(1)
    expect(state.currentIndex).toBe(0)
  })

  it('SET_USER_IN sets userIn to true', () => {
    const state = canvasReducer(initialState, {
      type: 'SET_USER_IN',
      in: true
    })
    expect(state.userIn).toBe(true)
    expect(state.history).toEqual([])
    expect(state.currentIndex).toBe(-1)
  })

  it('SET_USER_IN sets userIn to false', () => {
    const stateWithUser: State = {
      history: [],
      currentIndex: -1,
      savedDrawings: [],
      userIn: true
    }
    const state = canvasReducer(stateWithUser, {
      type: 'SET_USER_IN',
      in: false
    })
    expect(state.userIn).toBe(false)
  })

  it('SET_USER_IN preserves other state properties', () => {
    const stateWithData: State = {
      history: [{ prompt: 'test', commands: cmdA }],
      currentIndex: 0,
      savedDrawings: [mockSavedDrawing],
      userIn: false
    }
    const state = canvasReducer(stateWithData, {
      type: 'SET_USER_IN',
      in: true
    })
    expect(state.userIn).toBe(true)
    expect(state.history.length).toBe(1)
    expect(state.currentIndex).toBe(0)
    expect(state.savedDrawings.length).toBe(1)
  })
})