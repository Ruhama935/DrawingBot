import type { DrawCommand } from '../../models/DrawCommand'

export function renderCommands(
  ctx: CanvasRenderingContext2D,
  commands: DrawCommand[]
) {
  if (!ctx) return

  ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height)

  if (!commands || commands.length === 0) return

  for (const cmd of commands) {
    if (!cmd || !cmd.type) continue

    switch (cmd.type) {
      case 'circle':
        ctx.beginPath()
        ctx.arc(cmd.x, cmd.y, cmd.radius, 0, Math.PI * 2)
        ctx.fillStyle = cmd.color || 'black'
        ctx.fill()
        break

      case 'line':
        ctx.beginPath()
        ctx.moveTo(cmd.from.x, cmd.from.y)
        ctx.lineTo(cmd.to.x, cmd.to.y)
        ctx.strokeStyle = cmd.color || 'black'
        ctx.lineWidth = cmd.width || 2
        ctx.stroke()
        break

      case 'rect':
        ctx.fillStyle = cmd.color || 'black'
        ctx.fillRect(cmd.x, cmd.y, cmd.width, cmd.height)
        break

      default:
        break
    }
  }
}
