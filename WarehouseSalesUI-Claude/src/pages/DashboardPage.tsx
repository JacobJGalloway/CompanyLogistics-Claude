import Header from '../components/Header'
import InventoryViewer from '../components/InventoryViewer'

export default function DashboardPage() {
  return (
    <>
      <Header />
      <div style={{ padding: '1rem' }}>
        <InventoryViewer />
      </div>
    </>
  )
}
