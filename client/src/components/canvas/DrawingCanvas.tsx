import { useRef, useEffect } from 'react'
import { renderCommands } from './canvasEngine'
import { useCanvas } from '../../contexts/CanvasContext'

export default function DrawingCanvas() {
    const { commands } = useCanvas()

    const canvasRef = useRef<HTMLCanvasElement>(null)

    useEffect(() => {
        const canvas = canvasRef.current
        if (!canvas) return

        const ctx = canvas.getContext('2d')
        if (!ctx) return

        renderCommands(ctx, commands)
    }, [commands])

    return (
        <div style={{ padding: 16 }}>
            <canvas className="drawing-canvas"
                ref={canvasRef}
                width={900}
                height={460} />
        </div>
    )
}   
