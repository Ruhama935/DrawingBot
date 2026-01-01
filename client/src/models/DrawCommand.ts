export type DrawCommand =
  | {
      type: 'circle'
      x: number
      y: number
      radius: number
      color?: string
    }
  | {
      type: 'line'
      from: { x: number; y: number }
      to: { x: number; y: number }
      width?: number
      color?: string
    }
  | {
      type: 'rect'
      x: number
      y: number
      width: number
      height: number
      color?: string
    }
    
