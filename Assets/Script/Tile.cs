using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Vector3 firstPosition;
    private Vector3 finalPosition;
    private float swipeAngle;
    private Vector3 tempPosition;

    //Menampung data posisi tile
    public float xPosition;
    public float yPosition;
    public int column;
    public int row;
    private Grid grid;
    private GameObject otherTile;
    public bool isMatch = false;
    private int previousColumn;
    private int previousRow;

    // Start is called before the first frame update
    void Start()
    {
        //Menentukan posisi dari tile
        grid = FindObjectOfType<Grid>();
        xPosition = transform.position.x;
        yPosition = transform.position.y;
        column = Mathf.RoundToInt((xPosition - grid.startPosition.x) / grid.Offset.x);
        row = Mathf.RoundToInt((yPosition - grid.startPosition.y) / grid.Offset.x);
        Debug.Log(row+", "+column);
    }

    // Update is called once per frame
    void Update()
    {
        CheckMatches();
        xPosition = (column * grid.Offset.x) + grid.startPosition.x;
        yPosition = (row * grid.Offset.y) + grid.startPosition.y;
        SwipeTile();
    }

    void OnMouseDown()
    {
        //Mendapatkan titik awal sentuhan jari  
        firstPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp()
    {
        //Mendapatkan titik akhir sentuhan jari
        finalPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    void CalculateAngle()
    {
        //Menghitung sudut antara posisi awal dan posisi akhir
        swipeAngle = Mathf.Atan2(finalPosition.y - firstPosition.y, finalPosition.x - firstPosition.x) * 180 / Mathf.PI;
        MoveTile();
    }

    void SwipeTile()
    {
        if (Mathf.Abs(xPosition - transform.position.x) > .1)
        {
            //Move towards the target
            tempPosition = new Vector2(xPosition, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(xPosition, transform.position.y);
            transform.position = tempPosition;
            grid.tiles[column, row] = this.gameObject;
        }

        if (Mathf.Abs(yPosition - transform.position.y) > .1)
        {
            //Move towards the target
            tempPosition = new Vector2(transform.position.x, yPosition);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(transform.position.x, yPosition);
            transform.position = tempPosition;
            grid.tiles[column, row] = this.gameObject;
        }
    }

    void MoveTile()
    {
        if (swipeAngle > -45 && swipeAngle <= 45)
        {
            //Right swipe
            SwipeRightMove();
        }
        else if (swipeAngle > 45 && swipeAngle <= 135)
        {
            //Up swipe
            SwipeUpMove();
        }
        else if (swipeAngle > 135 || swipeAngle <= -135)
        {
            //Left swipe
            SwipeLeftMove();
        }
        else if (swipeAngle < -45 && swipeAngle >= -135)
        {
            //Down swipe
            SwipeDownMove();
        }
    }

    void SwipeRightMove()
    {
        //Menukar posisi dengan sebelah kanan nya
        otherTile = grid.tiles[column + 1, row];
        otherTile.GetComponent<Tile>().column -= 1;
        column += 1;
    }

    void SwipeUpMove()
    {
        //Menukar posisi dengan sebelah atasnya
        otherTile = grid.tiles[column, row + 1];
        otherTile.GetComponent<Tile>().row -= 1;
        row += 1;
    }

    void SwipeLeftMove()
    {
        //Menukar posisi dengan sebelah kirinya
        otherTile = grid.tiles[column - 1, row];
        otherTile.GetComponent<Tile>().column += 1;
        column -= 1;
    }

    void SwipeDownMove()
    {
        //Menukar posisi denhgan sebelah bawahnya
        otherTile = grid.tiles[column, row - 1];
        otherTile.GetComponent<Tile>().row += 1;
        row -= 1;
    }

    void CheckMatches()
    {
        if (column > 0 && column < grid.gridSizeX - 1)
        {        
            GameObject leftTile = grid.tiles[column - 1, row];
            GameObject rightTile = grid.tiles[column + 1, row];
            if (leftTile != null && rightTile != null)
            {
                if (leftTile.CompareTag(gameObject.tag) && rightTile.CompareTag(gameObject.tag))
                {
                    Debug.Log("Match!");
                    isMatch = true;
                    rightTile.GetComponent<Tile>().isMatch = true;
                    leftTile.GetComponent<Tile>().isMatch = true;
                }
            }
        }

        if (row > 0 && row < grid.gridSizeY - 1)
        {
            GameObject upTile = grid.tiles[column, row + 1];
            GameObject downTile = grid.tiles[column, row - 1];
            if (upTile != null && downTile != null)
            {
                if (upTile.CompareTag(gameObject.tag) && downTile.CompareTag(gameObject.tag))
                {
                    isMatch = true;
                    downTile.GetComponent<Tile>().isMatch = true;
                    upTile.GetComponent<Tile>().isMatch = true;
                }
            }
        }

        if (isMatch)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.color = Color.grey;
            StartCoroutine(checkMove());
        }

    }

    IEnumerator checkMove()
    {
        yield return new WaitForSeconds(.5f);
        //Cek jika tile nya tidak sama kembalikan, jika ada yang sama panggil DestroyMatches
        if (otherTile != null)
        {
            if (!isMatch && !otherTile.GetComponent<Tile>().isMatch)
            {
                otherTile.GetComponent<Tile>().row = row;
                otherTile.GetComponent<Tile>().column = column;
                row = previousRow;
                column = previousColumn;
            }
            else
            {
                grid.DestroyMatches();
            }
        }
        otherTile = null;
    }


}