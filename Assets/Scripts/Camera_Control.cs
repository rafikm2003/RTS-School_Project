using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Camera_Control : MonoBehaviour
{
List<Unit> selectedUnits = new List<Unit>();

public float cameraSpeed;
public float zoomSpeed;
public float groundHeight;
public Vector2 cameraHeightMinMax;
public Vector2 cameraRotationMinMax;
[Range(0,1)]
public float zoomLerp = 0.1f;
[Range(0,0.2f)]
public float cursorTreshold;
Vector2 mousePos;    
Vector2 mousePosScreen;
Vector2 keyboardInput;
Vector2 mouseScroll; 
bool isCursorInGameScreen;
Rect selectionRect, boxRect;
new Camera camera; 
RectTransform selectionBox;
private void Awake() 
{
selectionBox = GetComponentInChildren<Image>(true).transform as RectTransform; 
camera = GetComponent<Camera>();
selectionBox.gameObject.SetActive(false); 
}
 private void Update() 
{
    UpdateMovement();
    UpdateZoom();
    UpdateClicks();


}









void UpdateMovement()
{
    keyboardInput = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
    mousePos = Input.mousePosition;
    mousePosScreen = camera.ScreenToViewportPoint(mousePos);
    isCursorInGameScreen = mousePosScreen.x >=0 && mousePosScreen.x <=1 &&
    mousePosScreen.y>=0 && mousePosScreen.y <=1;
    Vector2 movementDirection = keyboardInput;
    if(isCursorInGameScreen) 
    {
        if (mousePosScreen.x < cursorTreshold) movementDirection.x -= 1- mousePosScreen.x / cursorTreshold;
        if (mousePosScreen.x > cursorTreshold) movementDirection.x += 1- (1-mousePosScreen.x) / (cursorTreshold);
        if (mousePosScreen.y < cursorTreshold) movementDirection.y -= 1- mousePosScreen.y / cursorTreshold;
        if (mousePosScreen.y > cursorTreshold) movementDirection.y += 1- (1-mousePosScreen.y) / (cursorTreshold);
        
        
        //nie do konca
          //  imageBox.localPosition = mousePos;
       
    var deltaPosition = new Vector3(movementDirection.x,0,movementDirection.y);
    deltaPosition *= cameraSpeed * Time.deltaTime;
    transform.position += deltaPosition;
}
}


void UpdateZoom()
{
    mouseScroll = Input.mouseScrollDelta;
    float zoomDelta = mouseScroll.y * zoomSpeed * Time.deltaTime;
    zoomLerp = Mathf.Clamp01(zoomLerp + zoomDelta);
    
    var position = transform.localPosition;
    position.y = Mathf.Lerp(cameraHeightMinMax.x,cameraHeightMinMax.y,zoomLerp) + groundHeight;
    transform.localPosition = position;

    var rotation = transform.localEulerAngles;
    rotation.x = Mathf.Lerp(cameraRotationMinMax.y,cameraRotationMinMax.x, zoomLerp);
    transform.localEulerAngles = rotation;


}
void UpdateClicks()
{
if(Input.GetMouseButtonDown(0))
{
    selectionBox.gameObject.SetActive(true); 
    selectionRect.position = mousePos;

}
else if (Input.GetMouseButtonUp(0)) 
{
selectionBox.gameObject.SetActive(false); 

}
if(Input.GetMouseButton(0))
{
    selectionRect.size = mousePos - selectionRect.position;
    boxRect = AbsRect(selectionRect);
    selectionBox.anchoredPosition = boxRect.position;
    selectionBox.sizeDelta = boxRect.size;
    UpdateSelecting();
}
if (Input.GetMouseButtonDown(1))
{
    GiveCommnds();
}




}

Ray ray;
RaycastHit rayHit;

[SerializeField]
LayerMask commandLayerMask=-1;

void GiveCommnds()
{
    ray = camera.ViewportPointToRay(mousePosScreen);
    if(Physics.Raycast(ray,out rayHit,1000,commandLayerMask));
    {
       object commandData = null;
        if(rayHit.collider is TerrainCollider)
        {
          // Debug.Log("Terrain " + rayHit.point.ToString()) ;
            commandData=rayHit.point;
        }
        else
        {
       //    Debug.Log(rayHit.collider);
        commandData = rayHit.collider.gameObject.GetComponent<Unit>();
        }
        GiveCommands(commandData);
    }
}

void GiveCommands(object dataCommand)
{
    foreach(Unit unit in selectedUnits)
        unit.SendMessage("Command",dataCommand,SendMessageOptions.DontRequireReceiver);

}














Rect AbsRect(Rect rect)
{
    if(rect.width<0)
    {
        rect.x += rect.width;
        rect.width *=-1;
    }
     if(rect.height<0)
    {
        rect.y += rect.height;
        rect.height *=-1;
    }
    return rect;

}
void UpdateSelecting()
{
    selectedUnits.Clear();
    foreach (Unit unit in Unit.SelectableUnits)
    {
        if (!unit || !unit.IsAlive) continue;
        var pos = unit.transform.position;
        var posScreen = camera.WorldToScreenPoint(pos);
        bool inRect = IsPointInRect(boxRect, posScreen);
        (unit as ISelectable).SetSelected(inRect);
        if(inRect)
        {
            selectedUnits.Add(unit);
        }

    }

}

bool IsPointInRect(Rect rect,Vector2 point)
{
    return point.x>=rect.position.x && point.x<=(rect.position.x + rect.size.x)&&
    point.y>=rect.position.y && point.y<=(rect.position.y + rect.size.y);
}

}