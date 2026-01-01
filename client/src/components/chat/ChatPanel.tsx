import { useState } from 'react'
import PromptInput from './PromptInput'
import { type ChatMessage } from '../../models/ChatMessage'

export default function ChatPanel() {
    const [messages, setMessages] = useState<ChatMessage[]>([])

    function addMessage(text: string, type: 'user' | 'system') {
        setMessages(prev => [
            ...prev,
            { id: crypto.randomUUID(), text, type }
        ])
    }

    return (
        <div className="chat-panel">
            <h2 className="chat-title">🎨 Draw Bot</h2>

            {/* הצגת הבועות */}
            <div className="chat-messages">
                {messages.map(m => (
                    <div key={m.id} className={`chat-bubble ${m.type}`}>
                        {m.text}
                    </div>
                ))}
            </div>

            <PromptInput
                onPromptSent={(text) => addMessage(text, 'user')}
                onSystemMessage={(text) => addMessage(text, 'system')}
            />
        </div>
    )
}
