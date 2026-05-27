import './Tabs.css'

function Tabs({ items = [], active, onChange }) {
  return (
    <div className="tabs">
      {items.map((item) => (
        <button
          key={item.value}
          type="button"
          className={`tab ${active === item.value ? 'active' : ''}`}
          onClick={() => onChange?.(item.value)}
        >
          {item.label}
        </button>
      ))}
    </div>
  )
}

export default Tabs
