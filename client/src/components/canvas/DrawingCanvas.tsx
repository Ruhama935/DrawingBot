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

        ctx.clearRect(0, 0, canvas.width, canvas.height)

        renderCommands(ctx, commands)
    }, [commands])

    return (
        <div style={{ padding: 16 }}>
            <canvas
                ref={canvasRef}
                width={900}
                height={460}
                style={{
                    background: 'white',
                    borderRadius: 8,
                    boxShadow: '0 4px 12px rgba(219, 23, 23, 0.08)'
                }} />
        </div>
    )
}
