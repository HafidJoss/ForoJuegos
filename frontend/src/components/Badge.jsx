import './Badge.css'

function Badge({ variant = 'info', className = '', ...props }) {
  return <span className={`badge badge-${variant} ${className}`.trim()} {...props} />
}

export default Badge
