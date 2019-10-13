using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int gridSizeX, gridSizeY;
    public GameObject tilePrefab;
    public GameObject[] candies;
    public GameObject[,] tiles;
    public Vector2 Offset{
        get {return offset;}
    }
    private Vector2 startPos;
    private int candiesIndex;
    public Vector2 startPosition
    {
        get{return startPos;}
    }
    private int tilePoint = 10;
    private int currentPointStreak = 0;
    private Vector2 offset;
    private string currentObjectTag;
    public string CurrentObjectTag
    {
        get {return currentObjectTag;}
    }
    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        tiles = new GameObject[gridSizeX,gridSizeY];
        offset = tilePrefab.GetComponent<SpriteRenderer>().bounds.size;
        Debug.Log(offset.x);
        Debug.Log(offset.y);
        startPos = transform.position + (Vector3.left * (offset.x * gridSizeX / 2)) +(Vector3.down * (offset.y * gridSizeY / 3));

        for (int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                Vector2 pos = new Vector2(startPos.x + (x * offset.x), startPos.y + (y * offset.y));

                GameObject backgroundTile = Instantiate(tilePrefab, pos,tilePrefab.transform.rotation);
                backgroundTile.transform.parent = transform;
                backgroundTile.name = "(" + x + ","+ y + ")";
                
                int index = preventMatchSpawn(100,x,y);

                GameObject candy = ObjectPooler.Instance.SpawnFromPool(index.ToString(), pos, Quaternion.identity);
                candy.name = "(" + x + ","+ y + ")";
                tiles[x,y] = candy;
            }
        }
    }

    private int preventMatchSpawn(int maxIteration, int x, int y)
    {
        candiesIndex = Random.Range(0,candies.Length);
        int i = 0;
        while (MatchesAt(x, y, candies[candiesIndex]) && i < maxIteration)
        {
            candiesIndex = Random.Range(0, candies.Length);
            i++;
        }
        return candiesIndex;
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        //Cek jika ada tile yang sama dengan dibawah dan samping nya
        if (column > 1 && row > 1)
        {
            if (tiles[column - 1, row].tag == piece.tag && tiles[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (tiles[column, row - 1].tag == piece.tag && tiles[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            //Cek jika ada tile yang sama dengan atas dan sampingnya
            if (row > 1)
            {
                if (tiles[column, row - 1].tag == piece.tag && tiles[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (tiles[column - 1, row].tag == piece.tag && tiles[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {

        //Destroy tile di indeks tertentu
        if (tiles[column, row].GetComponent<Tile>().isMatch)
        {
            GameObject gm = tiles[column, row];
            currentObjectTag = gm.tag;
            checkTagForAchievement(currentObjectTag);
            gm.SetActive(false);
            tiles[column, row] = null;
            currentPointStreak += tilePoint;
            
        }
    }

    private void checkTagForAchievement(string objectTag)
    {
        if(objectTag == "Cake")
            GameManager.instance.achievementSystem.OnNotify("Cake event");

        else if(objectTag == "Cookies")
            GameManager.instance.achievementSystem.OnNotify("Cookie event");

        else if(objectTag == "Star")
            GameManager.instance.achievementSystem.OnNotify("Star event");
    }

    public void DestroyMatches()
    {
        //Lakukan looping untuk cek tile yang null lalu di Destroy
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                if (tiles[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        GameManager.instance.SetScore(currentPointStreak);
        currentPointStreak = 0;
        StartCoroutine(DecreaseRow());
    }

    private IEnumerator FillBoard()
    {
        //Isi board kembali
        RefillBoard();
        yield return new WaitForSeconds(.5f);
        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
    }

    private void RefillBoard(){
        //Lakukan looping untuk mengecek jika ada tile yang kosong, isi lagi
        for(int i=0; i<gridSizeX; i++){
            for(int j=0; j<gridSizeY; j++){
                if(tiles[i,j] == null){
                    Vector2 tempPosition = new Vector2(startPos.x + (i * offset.x), startPos.y + (j * offset.y));
                    int candyToUse = Random.Range(0, candies.Length);
                    GameObject tile = ObjectPooler.Instance.SpawnFromPool(candyToUse.ToString(), tempPosition, Quaternion.identity);
                   tiles[i, j] = tile;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        //Check jika tile yang diisi ada yang matching
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                if (tiles[i, j] != null)
                {
                    if (tiles[i, j].GetComponent<Tile>().isMatch)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator DecreaseRow()
    {
        //Lakukan pengurangan row
        int nullCount = 0;
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                if (tiles[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    tiles[i, j].GetComponent<Tile>().row -= nullCount;
                    tiles[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoard());
    }

}
