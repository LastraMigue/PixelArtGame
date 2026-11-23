# PixelArtGame — Escenario de Ruinas 2D

Proyecto de Unity 2D desarrollado como entrega de la actividad **"Diseño de escenario en pixel art para videojuego de scroll lateral"**.

---

## Sobre el proyecto

### La actividad

El objetivo era diseñar un escenario original en pixel art para un videojuego 2D de scroll lateral, con un recorrido claro y un estilo visual coherente, cumpliendo los siguientes requisitos:

- **Temática libre** con tres planos separados: fondo lejano, fondo intermedio y terreno jugable.
- **Cinco elementos decorativos** integrados en el entorno.
- **Recorrido con inicio y final reconocibles**, cambios de altura y al menos un punto de interés.
- **Silueta de personaje** como referencia de escala.
- Misma escala de píxel, perspectiva y dirección de iluminación en todos los elementos.
- Sin desenfoques, suavizado de bordes ni escalados que deformen los píxeles.

### Lo que se ha implementado

La temática elegida es una **ruinas antiguas** con vegetación creciendo entre la piedra. El escenario cuenta con:

| Plano            | Archivo                 | Descripción                                   |
| ---------------- | ----------------------- | ---------------------------------------------- |
| Fondo lejano     | `Far_Background.png`  | Cielo con niebla y columnas de ruinas al fondo |
| Fondo intermedio | `Midground.png`       | Muros derruidos y vegetación media distancia  |
| Terreno jugable  | `Playable_Ground.png` | Plataformas de piedra con cambios de altura    |

**Cinco elementos decorativos** presentes en el escenario:

1. Columnas de piedra rotas
2. Vegetación (enredaderas / musgo)
3. Antorchas encendidas
4. Bloques caídos / escombros
5. Arco de entrada al final del recorrido

**Inicio y final** marcados en `POI_Start_End.png`, con el arco como punto de llegada reconocible.

---

## Mecánicas implementadas

### Movimiento y control del personaje

El jugador se mueve con las flechas o `A/D` y salta con `Espacio`. La física usa `Rigidbody2D` con `gravityScale = 3` para una caída más rápida y arcade. Se elimina la fricción lateral (`PhysicsMaterial2D friction = 0`) para evitar que el personaje se quede pegado a las paredes.

### Reaparición al caer (respawn)

En [`Assets/Scripts/PlayerController.cs`](Assets/Scripts/PlayerController.cs), en el método `Update()`, se comprueba en cada fotograma si el jugador ha caído por debajo del eje Y configurado como `deathY = -3f`. Si lo supera, el personaje reaparece instantáneamente en su posición inicial sin recargar la escena:

```csharp
if (transform.position.y < deathY)
{
    rb.position = start;
    Vel = Vector2.zero;
}
```

> No se reinicia la escena: solo se teletransporta al punto de inicio y se cancela la velocidad, lo que hace la reaparición imperceptible y sin pantalla de carga.

### Final con mensaje — Trigger "Finish"

Al llegar al arco del final, el personaje toca un **Trigger** con el tag `Finish`. Esto activa el método `OnTriggerEnter2D`, que:

1. Bloquea el movimiento del jugador (`won = true`).
2. Muestra el mensaje **"NIVEL SUPERADO!!!"** centrado en pantalla con `OnGUI()`.
3. Espera 3 segundos y cierra el juego (o detiene el Play Mode en el Editor).

```csharp
void OnTriggerEnter2D(Collider2D other)
{
    if (!other.CompareTag("Finish")) return;
    won = true;
    Invoke(nameof(End), 3f);
}
```

### Colisiones y límites del mapa

Las superficies pisables del terreno (`Playable_Ground`) usan **Polygon Collider 2D** para adaptarse con precisión al contorno irregular de la piedra en pixel art, sin rectángulos que "floten" sobre los bordes del sprite.

Los bordes del nivel (izquierda, derecha, suelo invisible) se delimitan con **Box Colliders 2D** para impedir que el jugador salga del escenario lateralmente. La cámara también se restringe mediante código para no mostrar más allá de los límites del nivel:

```csharp
float half = cam.orthographicSize * cam.aspect;
float x = Mathf.Clamp(transform.position.x, half, levelWidth - half);
cam.transform.position = new Vector3(Mathf.Round(x * 16f) / 16f, camY, -10f);
```

> La posición de la cámara se redondea a la cuadrícula de 16 px para evitar el "shimmer" (vibración subpíxel) característico del pixel art.

---

## Estructura del repositorio

```
PixelArtGame/
├── Assets/
│   ├── Art/                        ← Sprites del escenario
│   │   ├── Far_Background.png      ← Plano lejano
│   │   ├── Midground.png           ← Plano intermedio
│   │   ├── Playable_Ground.png     ← Terreno jugable
│   │   ├── Player_Scale_Ref.png    ← Silueta de referencia de escala
│   │   ├── POI_Start_End.png       ← Inicio y final del recorrido
│   │   ├── Props_5.png             ← Hoja con los 5 elementos decorativos
│   │   ├── ruins_scene.png         ← Escenario completo en PNG original
│   │   ├── ruins_spritesheet.png   ← Tileset y decoraciones con transparencia
│   │   └── ruins_layers.psd        ← Archivo editable con capas nombradas
│   └── Scripts/
│       └── PlayerController.cs     ← Movimiento, respawn, cámara y final
├── Packages/
│   ├── manifest.json               ← Paquetes 2D del proyecto
│   └── packages-lock.json
├── ProjectSettings/                ← Capas, Physics 2D, calidad, cámara...
├── .gitignore
└── README.md
```

> **Sobre los archivos de entrega:** los tres archivos requeridos están en `Assets/Art/`: `ruins_scene.png` (escenario completo en PNG), `ruins_spritesheet.png` (tileset con transparencia) y `ruins_layers.psd` (archivo editable con capas nombradas).

---

## Herramientas necesarias

| Herramienta                      | Versión                      | Para qué                                        |
| -------------------------------- | ----------------------------- | ------------------------------------------------ |
| **Unity Hub**              | Última estable               | Gestionar versiones de Unity y abrir el proyecto |
| **Unity Editor**           | 6000.x (Unity 6) o compatible | Motor del juego                                  |
| **Git**                    | Cualquier versión reciente   | Clonar el repositorio                            |
| **Git LFS** *(opcional)* | —                            | Solo si se añaden assets binarios grandes       |

> No se necesita Visual Studio, Rider ni ningún IDE para ejecutar el proyecto. Solo se requieren Unity Hub + Unity Editor.

---

## Instalación paso a paso

### 1. Instalar Unity Hub y Unity Editor

1. Descarga **Unity Hub** desde [unity.com/download](https://unity.com/download) e instálalo.
2. Dentro de Unity Hub, ve a **Installs → Install Editor** y selecciona la versión **6000.x (Unity 6)** (o la que indique el proyecto al abrirlo).
3. En los módulos del instalador, marca al menos:
   - `Windows Build Support (IL2CPP)` *(si quieres exportar a exe)*
   - No hace falta instalar Visual Studio si solo quieres ejecutarlo.

### 2. Clonar o descargar el repositorio

**Opción A — Git (recomendado):**

```bash
git clone <URL_DEL_REPO>
```

Elige una ruta **sin espacios ni caracteres especiales** (ej: `C:\Proyectos\PixelArtGame`).

**Opción B — ZIP:**
Descarga el ZIP desde GitHub (`Code → Download ZIP`) y extráelo en una carpeta local.

**Verificación:** dentro de la carpeta debes ver `Assets/`, `Packages/`, `ProjectSettings/` y `.gitignore`. **No debe aparecer `Library/` ni `Temp/`** — se generarán solas.

### 3. Añadir el proyecto a Unity Hub

1. Abre **Unity Hub**.
2. En la pestaña **Projects**, haz clic en **Add → Add project from disk**.
3. Selecciona la **carpeta raíz** del proyecto (la que contiene `Assets/`).
4. Unity Hub detectará automáticamente la versión del Editor necesaria.

**Verificación:** el proyecto aparece en la lista de Unity Hub con la versión de Unity correcta.

### 4. Abrir el proyecto por primera vez

1. Haz clic en el proyecto en Unity Hub para abrirlo.
2. Unity importará todos los assets y descargará los paquetes 2D declarados en `Packages/manifest.json` (requiere conexión a internet la primera vez).
3. Este proceso puede tardar **entre 2 y 5 minutos** dependiendo del equipo. Es normal.

**Verificación:** al terminar se abre el Editor de Unity sin errores críticos en la consola. Las carpetas `Library/` y `Temp/` habrán aparecido en la carpeta del proyecto (generadas automáticamente).

### 5. Configurar el sistema de input (importante)

Este proyecto usa el **Input System clásico de Unity** (`Input.GetAxisRaw`). Dependiendo de tu instalación, Unity puede tener activado el nuevo Input System por defecto, lo que hace que los controles no respondan. Para evitarlo:

1. Ve a **Edit → Project Settings → Player**.
2. Abre la sección **Other Settings** y baja hasta **Active Input Handling**.
3. Cámbialo a **`Both`** (acepta tanto el sistema antiguo como el nuevo).
4. Unity pedirá reiniciar el editor — acepta con **Apply / Yes**.

> Si los controles no responden al pulsar Play, este es el ajuste a revisar primero.

### 6. Comprobar la escena

1. En la ventana **Project**, navega a `Assets/` y abre la escena principal (`.unity`).
2. **Haz clic dentro de la ventana Game** para que Unity capture el foco del teclado.
3. Pulsa ▶ **Play** para probar el juego.
3. Mueve el personaje con `←/→` o `A/D`, salta con `Espacio`.
4. Si caes al vacío, reapareces automáticamente en el inicio.
5. Al llegar al arco del final aparece el mensaje **"NIVEL SUPERADO!!!"**.

**Verificación final:**

- La consola no muestra errores en rojo.
- Los sprites se ven nítidos (sin suavizado, estilo pixel art limpio).
- La cámara sigue al personaje sin salirse de los límites del nivel.
- El trigger del final funciona correctamente.

---

## Solución de problemas frecuentes

| Problema                           | Causa probable                  | Solución                                                                          |
| ---------------------------------- | ------------------------------- | ---------------------------------------------------------------------------------- |
| Sprites con suavizado / borrosos   | Filtro de textura incorrecto    | Selecciona el sprite en Project → Inspector → `Filter Mode: Point (no filter)` |
| Paquetes 2D no se descargan        | Sin conexión a internet        | Conéctate y vuelve a abrir Unity                                                  |
| Error de versión de Unity         | Editor distinto al del proyecto | Instala la versión exacta desde Unity Hub                                         |
| Personaje no colisiona bien        | Collider no regenerado          | Selecciona `Playable_Ground` → Inspector → recalcula el Polygon Collider 2D    |
| Controles no responden al pulsar Play | Input Handling incorrecto    | `Edit → Project Settings → Player → Other Settings → Active Input Handling → Both` y reinicia el Editor |
| `Library/` aparece en git status | `.gitignore` no aplicado      | Ejecuta `git rm -r --cached Library/` y vuelve a hacer commit                    |
