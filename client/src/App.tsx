import { CanvasProvider } from './contexts/CanvasContext'
import MainLayout from './layout/MainLayout'
import DrawingPage from './pages/DrawingPage'

function App() {
  return (
    <MainLayout>
      <CanvasProvider>
        <DrawingPage />
      </CanvasProvider>
    </MainLayout>
  )
}

export default App
