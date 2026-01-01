import { useEffect, useState } from 'react'
import { saveDrawing, loadDrawing, loadDrawingByUserId } from '../../api/drawingApi'
import { useToast } from '../../contexts/ToastContext'
import { useCanvas } from '../../contexts/CanvasContext'
import { validate as isUuid } from 'uuid'

export function useSaveLoadControls() {
    const {
        commands,
        prompts,
        savedDrawings,
        applyNewCommands,
        applyNewSavedDrawings,
        clear,
        setUserIn
    } = useCanvas()

    const toast = useToast()

    const [drawingId, setDrawingId] = useState('')
    const [isValidDrawingId, setIsValidDrawingId] = useState(false)
    const [selectedUser, setSelectedUser] = useState<string>('')

    async function handleSave() {
        try {
            const id = await saveDrawing(selectedUser, commands, prompts)
            toast(`הציור נשמר (ID: ${id})`, 'success')
            applyNewSavedDrawings([...savedDrawings, {prompt: prompts, commands}])
        } catch {
            toast('שמירה נכשלה', 'error')
        }
    }

    async function handleLoad() {
        try {
            const data = await loadDrawing(drawingId)
            clear()
            applyNewCommands(data.commands, data.prompt)
            toast('הציור נטען', 'success')
        } catch {
            toast('טעינה נכשלה', 'error')
        }
    }

    async function handleLoadByUserId() {
        try {
            const data = await loadDrawingByUserId(selectedUser)
            applyNewSavedDrawings(data)
            toast('נטען בהצלחה', 'success')
        } catch {
            toast('טעינה נכשלה', 'error')
        }
    }

    function handleChangeDrawingId(value: string) {
        setDrawingId(value)
        setIsValidDrawingId(isUuid(value))
    }

    useEffect(() => {
        if(!selectedUser)
            setUserIn(false)
        else {
            setUserIn(true)
            applyNewSavedDrawings([])
            clear()
        }
    }, [selectedUser])

    return {
        // state
        drawingId,
        isValidDrawingId,
        selectedUser,
        savedDrawings,
        commands,

        // setters
        setSelectedUser,
        handleChangeDrawingId,

        // actions
        handleSave,
        handleLoad,
        handleLoadByUserId
    }
}
