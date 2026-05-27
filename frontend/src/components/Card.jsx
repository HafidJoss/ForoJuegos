import './Card.css'

function Card({ className = '', ...props }) {
  return <div className={`card ${className}`.trim()} {...props} />
}

export default Card
