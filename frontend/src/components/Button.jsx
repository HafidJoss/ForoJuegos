import './Button.css'

const variants = {
  primary: 'btn btn-primary',
  secondary: 'btn btn-secondary',
  ghost: 'btn btn-ghost',
}

function Button({ variant = 'primary', className = '', ...props }) {
  return <button className={`${variants[variant]} ${className}`.trim()} {...props} />
}

export default Button
