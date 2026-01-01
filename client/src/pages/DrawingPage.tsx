import DrawingCanvas from "../components/canvas/DrawingCanvas";
import ChatPanel from "../components/chat/ChatPanel";
import SaveLoadControls from "../components/controls/SaveLoadControls";
import UndoRedoControls from "../components/controls/UndoRedoControls";

export default function DrawingPage() {
  return (
    <>
      <ChatPanel />
      <div>
        <UndoRedoControls />
        <SaveLoadControls />
        <DrawingCanvas />
      </div>
    </>
  )
}
