# Guía de UI / Frontend – Comunidad Gamer (MVP)

**Versión:** 1.0  
**Fecha:** 2026-05-19  
**Objetivo:** Definir lineamientos visuales para el frontend: tipografías, colores, estilos, componentes y consistencia.

---

## 1. Identidad visual

### 1.1 Estilo general
- **Estética:** moderna, gamer, limpia.
- **Tono:** oscuro con acentos vibrantes.
- **Sensación:** tecnológica, dinámica, comunitaria.

---

## 2. Tipografía

### 2.1 Fuente principal
- **UI / Texto general:** `Inter`  
  - Peso: 400 (regular), 500 (medium), 600 (semibold), 700 (bold)

### 2.2 Fuente alternativa (títulos gamer)
- **Títulos principales:** `Orbitron` (opcional)
  - Usar en H1/H2 o secciones clave

### 2.3 Tamaños base
- **Body:** 16px  
- **Small:** 14px  
- **H1:** 32px  
- **H2:** 24px  
- **H3:** 20px  
- **H4:** 18px  

---

## 3. Paleta de colores

### 3.1 Primarios
- **Primary:** #6C5CE7 (violeta neón)  
- **Primary Hover:** #5A4BD6  

### 3.2 Secundarios
- **Secondary:** #00D2D3 (cyan)  
- **Secondary Hover:** #00B5B6  

### 3.3 Neutrales (modo oscuro)
- **Background:** #0F1117  
- **Surface:** #171A22  
- **Surface 2:** #1F2430  
- **Border:** #2A2F3A  

### 3.4 Texto
- **Text Primary:** #E6E8EF  
- **Text Secondary:** #AAB1C5  
- **Text Muted:** #7C8296  

### 3.5 Estados
- **Success:** #2ECC71  
- **Warning:** #F1C40F  
- **Error:** #E74C3C  
- **Info:** #3498DB  

---

## 4. Espaciado y layout

### 4.1 Grid
- **Base:** 8px  
- Espaciados: 4, 8, 12, 16, 24, 32, 48, 64

### 4.2 Contenedor
- **Max width:** 1200px  
- **Padding horizontal:** 24px  

---

## 5. Componentes principales

### 5.1 Botones
- **Primary:** fondo primary, texto blanco, borde 0, radius 8px  
- **Secondary:** fondo secondary, texto blanco  
- **Ghost:** fondo transparente, borde 1px, color primary  

Estados:
- Hover: +10% brillo  
- Disabled: opacidad 0.5  

### 5.2 Cards
- Fondo: Surface  
- Radius: 12px  
- Sombra: suave (0 4px 12px rgba(0,0,0,0.25))

### 5.3 Inputs
- Fondo: Surface 2  
- Border: 1px solid Border  
- Radius: 8px  
- Focus: outline Primary  

### 5.4 Tabs
- Activa: texto primary + underline  
- Inactiva: text secondary  

### 5.5 Badges
- Fondos según estado: success/warning/error  
- Radius: 6px  
- Padding: 4px 8px  

---

## 6. Iconografía
- **Set recomendado:** Lucide Icons / FontAwesome  
- Estilo: lineal, consistente  

---

## 7. Tipos de páginas (MVP)
- Home (últimas reseñas, juegos trending)
- Catálogo de juegos
- Detalle de juego
- Perfil de usuario
- Publicación de reseña
- Subida de contenido UGC
- Moderación (admin)

---

## 8. Accesibilidad
- Contraste mínimo WCAG AA
- Tamaño mínimo de texto 14px
- Estados focus visibles

---

## 9. UI para legalidad UGC
- Banner claro en subida:
  - “Solo contenido con permiso legal”
- Checkbox obligatorio con aceptación
- Enlace a política de contenido

---

## 10. Responsive
- Mobile first
- Breakpoints:
  - sm: 640px  
  - md: 768px  
  - lg: 1024px  
  - xl: 1280px  

---

## 11. Tema claro (futuro)
- Definir paleta inversa en fase 2

---