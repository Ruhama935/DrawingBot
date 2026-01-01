import { useCanvas } from "../../contexts/CanvasContext"

export default function UndoRedoControls() {
    const { undo, redo, clear, canUndo, canRedo } = useCanvas()

    return (
        <div className="draw-toolbar">
            <button onClick={undo} disabled={!canUndo}>Undo</button>
            <button onClick={redo} disabled={!canRedo}>Redo</button>
            <button onClick={clear}>Clear</button>
        </div>
    )
}
