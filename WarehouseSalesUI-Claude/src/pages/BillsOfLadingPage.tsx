import { useState, useEffect } from 'react'
import { useApiClients } from '../hooks/useApiClients'
import type { BillOfLading, LineEntry } from '../types'

const cell: React.CSSProperties = {
  border: '1px solid var(--table-border)',
  padding: '0.4rem 0.8rem',
  background: 'var(--surface)',
  color: '#08060d',
}

const headCell: React.CSSProperties = {
  ...cell,
  fontWeight: 600,
  textAlign: 'left',
}

interface ModalState {
  transactionId: string
  entries: LineEntry[]
}

export default function BillsOfLadingPage() {
  const { logistics } = useApiClients()
  const [bols, setBols] = useState<BillOfLading[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [modal, setModal] = useState<ModalState | null>(null)
  const [modalLoading, setModalLoading] = useState(false)

  useEffect(() => {
    logistics.get<BillOfLading[]>('/api/BillOfLading')
      .then(setBols)
      .catch(() => setError('Failed to load bills of lading.'))
      .finally(() => setLoading(false))
  }, [])

  async function openModal(transactionId: string) {
    setModalLoading(true)
    setModal({ transactionId, entries: [] })
    try {
      const entries = await logistics.get<LineEntry[]>(`/api/BillOfLading/${transactionId}/line-entry`)
      setModal({ transactionId, entries })
    } catch {
      setModal({ transactionId, entries: [] })
    } finally {
      setModalLoading(false)
    }
  }

  return (
    <main style={{ padding: '1rem', textAlign: 'left' }}>
      <h2>Bills of Lading</h2>

      {loading && <p>Loading...</p>}
      {error && <p style={{ color: 'red' }}>{error}</p>}

      {!loading && !error && bols.length === 0 && (
        <p>No bills of lading found.</p>
      )}

      {bols.length > 0 && (
        <table style={{ borderCollapse: 'collapse', width: '100%', marginTop: '1rem' }}>
          <thead>
            <tr>
              {['Transaction ID', 'Status', 'Customer', 'Destination', ''].map(h => (
                <th key={h} style={headCell}>{h}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {bols.map(bol => (
              <tr key={bol.partitionKey}>
                <td style={cell}>{bol.transactionId}</td>
                <td style={cell}>{bol.status}</td>
                <td style={cell}>{bol.customerFirstName} {bol.customerLastName}</td>
                <td style={cell}>{bol.city}, {bol.state}</td>
                <td style={{ ...cell, textAlign: 'center' }}>
                  <button onClick={() => openModal(bol.transactionId)}>View</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {modal && (
        <div style={{
          position: 'fixed', inset: 0,
          background: 'rgba(0,0,0,0.5)',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          zIndex: 200,
        }}>
          <div style={{
            background: 'var(--bg)',
            border: '1px solid var(--border)',
            borderRadius: '8px',
            padding: '1.5rem',
            minWidth: '560px',
            maxWidth: '90vw',
            maxHeight: '80vh',
            overflowY: 'auto',
            position: 'relative',
          }}>
            <button
              onClick={() => setModal(null)}
              style={{
                position: 'absolute', top: '0.75rem', right: '0.75rem',
                background: 'none', border: 'none', cursor: 'pointer',
                fontSize: '1.2rem', color: 'var(--text-h)',
              }}
              aria-label="Close"
            >✕</button>

            <h2 style={{ marginTop: 0, marginBottom: '1rem' }}>
              Line Entries — {modal.transactionId}
            </h2>

            {modalLoading && <p>Loading...</p>}

            {!modalLoading && modal.entries.length === 0 && (
              <p>No line entries found.</p>
            )}

            {!modalLoading && modal.entries.length > 0 && (
              <table style={{ borderCollapse: 'collapse', width: '100%' }}>
                <thead>
                  <tr>
                    {['Location ID', 'SKU', 'Qty', 'Processed', 'Processed Date'].map(h => (
                      <th key={h} style={headCell}>{h}</th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {modal.entries.map(entry => (
                    <tr key={entry.partitionKey}>
                      <td style={cell}>{entry.locationId}</td>
                      <td style={cell}>{entry.skuMarker}</td>
                      <td style={cell}>{entry.quantity}</td>
                      <td style={cell}>{entry.isProcessed ? 'Yes' : 'No'}</td>
                      <td style={cell}>
                        {entry.processedDate
                          ? new Date(entry.processedDate).toLocaleDateString()
                          : '—'}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}

            <div style={{ marginTop: '1rem', textAlign: 'right' }}>
              <button onClick={() => setModal(null)}>Close</button>
            </div>
          </div>
        </div>
      )}
    </main>
  )
}
