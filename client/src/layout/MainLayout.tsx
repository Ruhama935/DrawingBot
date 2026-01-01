import type { ReactNode } from 'react'
import '../styles/layout.css'

export default function MainLayout({ children }: { children: ReactNode }) {
  return <div className="main-layout">{children}</div>
}
