using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GridGenerator : MonoBehaviour
{




    [SerializeField]
    GameObject _tile = null;
    [SerializeField]
    Vector2 _grid_size = new Vector2(10, 10);

    [SerializeField]
    Text _color_text;
    [SerializeField]
    Text _name_text;
    [SerializeField]
    Text _position_text;
    [SerializeField]
    bool _setRandomColor = true;
    [SerializeField]
    Color _gridColor = new Color();




    GameObject _newTile = null;
    int _tileNumber = 1;



    void Start()
    {
        GenerateGridOnStart();
    }


    void Update()
    {
        GetInputForTileInfo();
    }

    #region
    void GenerateGridOnStart()
    {
        for (float i = 0; i < _grid_size.x; i++)
        {
            for (float j = 0; j < _grid_size.y; j++)
            {
                _newTile = Instantiate(_tile, new Vector3(i, 0, j), Quaternion.identity, transform);


                _newTile.name = "Tile_" + _tileNumber.ToString();
                _tileNumber++;

                Renderer rend = _newTile.GetComponent<Renderer>();

                rend.material = new Material(Shader.Find("Standard"));
                if (_setRandomColor)
                {
                    rend.material.color = new Color(
                                        Random.Range(0f, 1f),
                                        Random.Range(0f, 1f),
                                        Random.Range(0f, 1f)
                                );

                }
                else
                {
                    rend.material.color = _gridColor;
                }


                TileInfo currentTileInfo = _newTile.GetComponent<TileInfo>();
                currentTileInfo.tile_color = rend.material.color;
                currentTileInfo.tile_name = _newTile.name;
                currentTileInfo.tile_position = _newTile.transform.position;
            }

        }

    }

    void GetInputForTileInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100) && Input.GetMouseButtonDown(0))
        {
            TileInfo currentTileInfo = hit.transform.gameObject.GetComponent<TileInfo>();

            if (currentTileInfo)
            {
                _color_text.text = currentTileInfo.tile_color.ToString();
                _position_text.text = currentTileInfo.tile_position.ToString();
                _name_text.text = currentTileInfo.tile_name.ToString();
            }
        }
    }
    #endregion
}