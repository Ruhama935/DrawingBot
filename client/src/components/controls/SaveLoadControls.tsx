import UserDropdown from '../dropdown/UserDropdown'
import { SavedDrawingsDropdown } from '../dropdown/SavedDrawingsDropdown'
import { useSaveLoadControls } from './useSaveLoadControls'

export default function SaveLoadControls() {
    const {
        drawingId,
        isValidDrawingId,
        selectedUser,
        savedDrawings,
        commands,
        setSelectedUser,
        handleChangeDrawingId,
        handleSave,
        handleLoad,
        handleLoadByUserId
    } = useSaveLoadControls()

    return (
        <div className="draw-toolbar">
            <button
                onClick={handleSave}
                disabled={!commands.length || !selectedUser}
            >
                Save
            </button>

            <input
                placeholder="Drawing ID"
                value={drawingId}
                onChange={e => handleChangeDrawingId(e.target.value)}
            />

            <button
                onClick={handleLoad}
                disabled={!isValidDrawingId}
            >
                Load Drawing By Id
            </button>

            <button
                onClick={handleLoadByUserId}
                disabled={!selectedUser}
            >
                Load Drawings by User
            </button>

            <UserDropdown
                selectedUser={selectedUser}
                onChange={setSelectedUser}
            />

            <SavedDrawingsDropdown items={savedDrawings} />
        </div>
    )
}
