import { useEffect, useState } from 'react'
import type { CommandGroup } from '../../models/CommandGroup'
import { useCanvas } from '../../contexts/CanvasContext'


export function SavedDrawingsDropdown({ items }: { items: CommandGroup[] }) {
    const [selectedIndex, setSelectedIndex] = useState<number | ''>('')
    const { clear, applyNewCommands } = useCanvas()


    const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const index = Number(e.target.value)
        setSelectedIndex(index)
        clear()
        applyNewCommands(items[index].commands, items[index].prompt)
    }

    useEffect(() => {
        setSelectedIndex('')
    }, [items])

    return (
        <label className="select-wrapper">
            <select className="select-input" value={selectedIndex} onChange={handleChange}>
                <option value="" disabled>
                     בחר ציור 
                </option>

                {items.map((item, index) => (
                    <option key={index} value={index}>
                        {item.prompt}
                    </option>
                ))}
            </select>
        </label>
    )
}
