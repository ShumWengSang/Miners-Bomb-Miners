using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Roland
{
    public class GrenadeBomb : BombsParent
    {
        Transform ourTransform;
        Vector2 nextTilePos;

        bool invul = true;
        WaitForSeconds waitTime;
        public float wait;
        public Direction Direction
        {
            get { return theDirection; }
            set { 
                    theDirection = value;
                    SpriteRenderer sp = GetComponent<SpriteRenderer>();
                    switch (Direction)
                    {
                        case Direction.Up:
                            if (sp.sprite != null)
                            {
                                Offset = sp.sprite.bounds.extents;
                            }
                            MoveDirection = new Vector2(0, 1);
                            break;
                        case Direction.Down:
                            if (sp.sprite != null)
                            {
                                Offset = -sp.sprite.bounds.extents;
                                Offset -= Offset * 0.01f;

                            }
                            MoveDirection = new Vector2(0, -1);
                            break;
                        case Direction.Left:
                            if (sp.sprite != null)
                            {
                                Offset = -sp.sprite.bounds.extents;
                                Offset -= Offset * 0.01f;
                            }
                            MoveDirection = new Vector2(-1, 0);
                            break;
                        case Direction.Right:
                            if (sp.sprite != null)
                            {
                                Offset = sp.sprite.bounds.extents;
                            }
                            MoveDirection = new Vector2(1, 0);
                            break;
                        case Direction.Stop:
                            MoveDirection = new Vector2(0, 0);
                            break;
                    }
                }
        }

        Direction theDirection;

        public int speed = 5;

        Vector2 MoveDirection = new Vector2(0, 0);
        Vector3 Offset;

        protected override void Init()
        {
            theTileMap = TileMapInterfacer.Instance.TileMap;
            Vector2 tilePos = theTileMap.ConvertWorldToTile(transform.position);
            SetTilePos((int)tilePos.x, (int)tilePos.y);
            transform.position = theTileMap.ConvertTileToWorld(tilePos);

            AudioSource theSrc = gameObject.AddComponent<AudioSource>();
            theSrc.clip = theClipToPlayWhenExplode;
            theSrc.playOnAwake = false;
            ourTransform = transform;

            StartCoroutine(notInvul());
        }

        IEnumerator notInvul()
        {
            yield return waitTime;
            invul = false;
        }
        protected override void OnSpawn()
        {
            Init();
            waitTime = new WaitForSeconds(wait);
            invul = true;
        }

        public override void Update()
        {
            nextTilePos = MoveDirection + theTileMap.ConvertWorldToTile(ourTransform.position - Offset);

            // Debug.Log("Our tile pos is " + theTileMap.ConvertWorldToTile(transform.position));
            Block theNextBlock = theTileMap.theMap.GetTileAt(nextTilePos);
            if (theNextBlock is Noblock)
            {
                ourTransform.localPosition += new Vector3(MoveDirection.x, MoveDirection.y, 0) * Time.deltaTime * speed;
            }
            else
            {
                if (!invul)
                {
                    Debug.Log("Exploding invul is " + invul);
                    //EXPLODE
                    Vector2 tilePos = theTileMap.ConvertWorldToTile(transform.position);
                    SetTilePos((int)tilePos.x, (int)tilePos.y);
                    transform.position = theTileMap.ConvertTileToWorld(tilePos);
                    Explode();
                }
            }
        }

        protected override void Explode()
        {
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    DigSpawnTile(i, j, BombPower);
                }
            }
            DigSpawnTile(x + 2, y, BombPower);
            DigSpawnTile(x - 2, y, BombPower);
            DigSpawnTile(x, y + 2, BombPower);
            DigSpawnTile(x, y - 2, BombPower);

            SpawnExplosion(x, y);
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }
}
