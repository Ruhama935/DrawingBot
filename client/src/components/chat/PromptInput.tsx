import { useState } from "react"
import { drawFromPrompt } from "../../api/drawingApi"
import { useCanvas } from '../../contexts/CanvasContext'

export default function PromptInput({
    onPromptSent,
    onSystemMessage
}: {
    onPromptSent: (text: string) => void
    onSystemMessage: (text: string) => void
}) {
    const { applyNewCommands, userIn } = useCanvas()
    const [text, setText] = useState('')
    const [loading, setLoading] = useState(false)

    async function handleDraw() {
        if (!text.trim()) return

        onPromptSent(text)   // 👈 בועת משתמש
        setLoading(true)

        try {
            const commands = await drawFromPrompt(text)
            
            if (commands.length > 0) {
                applyNewCommands(commands, text)
                onSystemMessage('הציור מוכן 🎨')
            } else {
                onSystemMessage('לא נמצאו פקודות לציור ℹ')
            }
        } catch {
            onSystemMessage('שגיאה ביצירת הציור ❌')
        } finally {
            setLoading(false)
            setText('')
        }
    }

    return (
        <div className="chat-input">
            <textarea
                value={text}
                onChange={e => setText(e.target.value)}
                placeholder="כתוב הוראה לציור..."
            />

            <button onClick={handleDraw} disabled={loading||!userIn}>
                {loading ? <div className="spinner" /> : 'צייר'}
            </button>
        </div>
    )
}
