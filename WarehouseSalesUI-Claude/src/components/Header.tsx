import { useState, useRef, useEffect } from 'react'
import { useAuth0 } from '@auth0/auth0-react'
import { UserCircle } from 'lucide-react'
import iconLight from '../assets/Digital Parts Icon Light Mode.svg'
import iconDark from '../assets/Digital Parts Icon Dark Mode.svg'
import nameLight from '../assets/Digital Parts Name Light Mode.svg'
import nameDark from '../assets/Digital Parts Name Dark Mode.svg'
import styles from './Header.module.css'

export default function Header() {
  const { logout } = useAuth0()
  const [open, setOpen] = useState(false)
  const wrapperRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (wrapperRef.current && !wrapperRef.current.contains(e.target as Node)) {
        setOpen(false)
      }
    }
    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [])

  return (
    <header className={styles.header}>
      <div className={styles.logo}>
        <picture>
          <source media="(prefers-color-scheme: dark)" srcSet={iconDark} />
          <img src={iconLight} alt="Digital Parts icon" className={styles.icon} />
        </picture>
        <picture>
          <source media="(prefers-color-scheme: dark)" srcSet={nameDark} />
          <img src={nameLight} alt="Digital Parts" className={styles.name} />
        </picture>
      </div>

      <div className={styles.profileWrapper} ref={wrapperRef}>
        <button
          className={styles.profileButton}
          onClick={() => setOpen(o => !o)}
          aria-label="Profile menu"
        >
          <UserCircle size={32} />
        </button>

        {open && (
          <div className={styles.dropdown}>
            <button
              className={styles.dropdownItem}
              onClick={() => logout({ logoutParams: { returnTo: window.location.origin + '/login' } })}
            >
              Log Out
            </button>
          </div>
        )}
      </div>
    </header>
  )
}
