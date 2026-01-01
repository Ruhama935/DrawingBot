import { type DrawCommand } from '../models/DrawCommand'

const BASE_URL = import.meta.env.VITE_API_URL;

export async function drawFromPrompt(prompt: string): Promise<DrawCommand[]> {
    const response = await fetch(`${BASE_URL}/generate`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            prompt
        })

    })

    if (!response.ok) {
        throw new Error('Failed to draw')
    }

    const data = await response.json()
    return data.commands
}

export async function saveDrawing(
  userId: string,  
  commands: DrawCommand[],
  prompt: string,
): Promise<string> {
  const res = await fetch(`${BASE_URL}/save`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      userId,
      prompt,
      commands
    })
  })

  if (!res.ok) throw new Error('Save failed')

  const data = await res.json()
  return data.drawingId
}

export async function loadDrawing(id: string): Promise<{commands: DrawCommand[], prompt: string}> {
  const res = await fetch(`${BASE_URL}/${id}`)

  if (!res.ok) throw new Error('Load failed')

  const data = await res.json()
  return data   
}

export async function loadDrawingByUserId(id: string): Promise<[{commands: DrawCommand[], prompt: string}]> {
  const res = await fetch(`${BASE_URL}/user/${id}`)

  if (!res.ok) throw new Error('Load failed')

  const data = await res.json()
  return data
}