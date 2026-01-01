import type { User } from '../../models/User'

const USERS_DATA: User[] = import.meta.env.VITE_USERS_JSON
    ? JSON.parse(import.meta.env.VITE_USERS_JSON)
    : []; 

export default function UserDropdown({ selectedUser, onChange }: { selectedUser: string; onChange: (userId: string) => void }) {
  return (
    <div>
      <label className="select-wrapper">
        <select
          className="select-input"
          value={selectedUser}
          onChange={(e) => onChange(e.target.value)}
        >
          <option value="" disabled>
            בחר משתמש 
          </option>
          {USERS_DATA.map((user) => (
            <option key={user.id} value={user.id}>
              {user.name}
            </option>
          ))}
        </select>
      </label>      
    </div>
  )
}
