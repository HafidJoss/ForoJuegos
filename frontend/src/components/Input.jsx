import './Input.css'

function Input({ label, id, className = '', ...props }) {
  const inputId = id || `input-${Math.random().toString(36).slice(2)}`

  return (
    <label className={`input-field ${className}`.trim()} htmlFor={inputId}>
      {label && <span>{label}</span>}
      <input id={inputId} {...props} />
    </label>
  )
}

export default Input
